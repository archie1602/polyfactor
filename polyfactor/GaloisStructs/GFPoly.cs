using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.Concurrent;

using polyfactor.Extensions;

namespace polyfactor.GaloisStructs
{
    public class GFPoly
    {
        public GF Field { get; private set; }
        private ulong[] _coeffs;
        private string _var = "x";

        /// <summary>
        /// returns polynomial variable
        /// </summary>
        public string Var
        {
            get => _var;

            set
            {
                // check if value isn't null or empty
                if (string.IsNullOrEmpty(value))
                    throw new ArgumentException();

                // check if value is letter [a-zA-Z] or greek letter
                if (!((value.Length == 1 && char.IsLetter(value[0])) || value.IsGreekLetter()))
                    throw new FormatException();

                _var = value;
            }
        }

        /// <summary>
        /// returns degree of the polynomial;
        /// if the polynomial is equal to zero, returns -1
        /// </summary>
        public int Deg { get; private set; }

        /// <summary>
        /// returns length of the pol (num of the coeffs in the pol)
        /// </summary>
        public int Length => _coeffs.Length;

        /// <summary>
        /// Leading coefficient of the polynomial
        /// </summary>
        public ulong LC => _coeffs[Length - 1];

        /// <summary>
        /// checks whether the polynomial is zero polynomial
        /// </summary>
        public bool IsZero => Deg == -1;

        /// <summary>
        /// checks whether the polynomial is one polynomial
        /// </summary>
        public bool IsOne => Deg == 0 && _coeffs[0] == 1UL;

        /// <summary>
        /// checks whether the polynomial is monic
        /// </summary>
        public bool IsMonic => LC == 1UL;

        /// <summary>
        /// checks whether the polynomial is square-free
        /// </summary>
        public bool IsSquareFree => GCD(this, Derivative(this)).IsOne;

        /// <summary>
        /// checks whether the polynomial is monomial
        /// </summary>
        public bool IsMonomial
        {
            get
            {
                // if the pol is a const
                if (Length == 1)
                    return false;

                for (int i = _coeffs.Length - 2; i >= 0; --i)
                    if (_coeffs[i] != 0UL)
                        return false;

                return true;
            }
        }

        #region Shorthand-Methods

        /// <summary>
        /// shorthand for writing zero polynomial: zero(x) = 0
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        public static GFPoly Zero(GF field) => CreatePol(field, new ulong[] { 0UL }, -1);

        /// <summary>
        /// shorthand for writing identity polynomial: one(x) = 1
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        public static GFPoly One(GF field) => CreatePol(field, new ulong[] { 1UL }, 0);

        #endregion

        #region Constructors

        /// <summary>
        /// creates polynomial from its string representation in infix format
        /// </summary>
        /// <param name="field"></param>
        /// <param name="polExpr"></param>
        public GFPoly(GF field, string polExpr)
        {
            if (field is null)
                throw new Exception("The galois field reference isn't set to an instance!");

            if (string.IsNullOrWhiteSpace(polExpr))
                throw new ArgumentException();

            polExpr = polExpr.Replace(" ", "");

            var polVar = ' ';

            // check pol for valid characters:
            // operations:      +,*,^
            // variables:       [a-zA-Z]
            // coefficients:    [0-9]
            for (int i = 0; i < polExpr.Length; ++i)
            {
                var currChar = polExpr[i];

                if (char.IsLetter(currChar))
                {
                    if (polVar == ' ')
                        polVar = currChar;
                    else if (currChar != polVar)
                        throw new FormatException();
                }
                else if (currChar != '+' &&
                         currChar != '*' &&
                         currChar != '^' &&
                         !char.IsDigit(currChar))
                    throw new FormatException();
            }

            // split pol by operation '+' => 5x^2+4x+6 goes to {"5x^2", "4x", "6"}
            var strPolTerms = polExpr.Split('+');

            // match case: 5x^2+4x+6+ => {"5x^2", "4x", "6", ""}  (for example)
            if (strPolTerms[strPolTerms.Length - 1] == string.Empty)
                throw new FormatException();

            // list of polynomial terms represented as a tuple of the form: (c, i) |-> c*x^i
            var terms = new List<(ulong coeff, int deg)>();

            // match case: +5x^2+4x+6 => {"", "5x^2", "4x", "6"}  (for example)
            int j = strPolTerms[0] == string.Empty ? 1 : 0;

            // expected monomials:
            // note: here x - is a 'polVar' variable
            // #1. c - integer number
            // #2. x - simple monomial
            // #3. cx^n
            // #4. c*x^n
            // #5. x^n
            // #6. c*x
            // #7. cx

            for (; j < strPolTerms.Length; ++j)
            {
                var currTerm = strPolTerms[j];

                // if operand (left or right) is lost
                if (currTerm == string.Empty)
                    throw new FormatException();

                // match case #1 - term is constant monomial
                if (ulong.TryParse(currTerm, out var constTerm))
                    terms.Add((constTerm, 0));
                else if (currTerm == char.ToString(polVar)) // match case #2 - x
                    terms.Add((1, 1));
                else
                {
                    // remaining cases:
                    // #3. cx^n
                    // #4. c*x^n
                    // #5. x^n
                    // #6. c*x
                    // #7. cx

                    // split monomial by operation '*'
                    var termParts = currTerm.Split('*');

                    // => possible cases:

                    if (termParts.Length == 1)
                    {
                        // match cases:

                        // #3. cx^n goes to {0 -> cx^n}
                        // #5. x^n  goes to {0 -> x^n}
                        // #7. cx   goes to {0 -> cx}

                        var currPart = termParts[0];

                        var monomDegParts = currPart.Split('^');

                        if (monomDegParts.Length == 1)
                        {
                            // match case: #7. cx

                            var cx = monomDegParts[0];

                            if (cx[cx.Length - 1] != polVar)
                                throw new FormatException();

                            if (!ulong.TryParse(cx.Substring(0, cx.Length - 1), out var monomCoeff))
                                throw new FormatException();

                            terms.Add((monomCoeff, 1));
                        }
                        else if (monomDegParts.Length == 2)
                        {
                            // match cases:
                            // #3. cx^n goes to {0 -> cx, 1 -> n}
                            // #5. x^n  goes to {0 -> x,  1 -> n}

                            var leftPart = monomDegParts[0];
                            var rightPart = monomDegParts[1];

                            // check: 1 -> n
                            if (!int.TryParse(rightPart, out var monomDeg))
                                throw new FormatException();

                            // check: 0 -> x
                            if (leftPart == char.ToString(polVar))
                                terms.Add((1, monomDeg));
                            else // check: 0 -> cx
                            {
                                var cx = leftPart;

                                if (cx[cx.Length - 1] != polVar)
                                    throw new FormatException();

                                if (!ulong.TryParse(cx.Substring(0, cx.Length - 1), out var monomCoeff))
                                    throw new FormatException();

                                terms.Add((monomCoeff, monomDeg));
                            }
                        }
                        else
                            throw new FormatException();
                    }
                    else if (termParts.Length == 2)
                    {
                        // match cases:

                        // #4. c*x^n goes to {0 -> c, 1 -> x^n}
                        // #6. c*x   goes to {0 -> c, 1 -> x}

                        var leftPart = termParts[0];
                        var rightPart = termParts[1];

                        // check 0 -> c
                        if (!ulong.TryParse(leftPart, out var monomCoeff))
                            throw new FormatException();

                        // possible cases: (right part)
                        // => 1 -> x
                        // => 1 -> x^n

                        var monomDegParts = rightPart.Split('^');

                        // possible case:
                        // => x goes to {0 -> x}
                        // => x^n goes to {0 -> x, 1 -> n}

                        // match case: {0 -> x}
                        if (monomDegParts.Length == 1 && monomDegParts[0] == char.ToString(polVar))
                            terms.Add((monomCoeff, 1));
                        else if (monomDegParts.Length == 2 && // match case: {0 -> x, 1 -> n}
                                 monomDegParts[0] == char.ToString(polVar) &&
                                 int.TryParse(monomDegParts[1], out var monomDeg))
                            terms.Add((monomCoeff, monomDeg));
                        else
                            throw new FormatException();
                    }
                    else
                        throw new FormatException();
                }
            }

            Comparison<(ulong coeff, int deg)> termComparator = (t1, t2) =>
            {
                if (t1.deg > t2.deg)
                    return -1;
                else if (t1.deg < t2.deg)
                    return 1;
                else
                    return 0;
            };

            // sort terms in desc order according to their degs
            terms.Sort(termComparator);

            int polLength = terms[0].deg + 1;

            var coeffs = new ulong[polLength];

            terms.ForEach(t => coeffs[t.deg] = (coeffs[t.deg] + t.coeff) % field.Order);

            // set polynomial variable, field, coeffs
            _var = polVar.ToString();
            Field = field;
            _coeffs = coeffs;

            RemoveLeadingZeros();
        }

        public GFPoly(GF field, ulong[] coeffs)
        {
            if (coeffs.Length == 0)
                throw new Exception("The coeff array must contain at least one element!");

            if (field is null)
                throw new Exception("The galois field reference isn't set to an instance!");

            Field = field;
            _coeffs = coeffs;

            for (int i = 0; i < coeffs.Length; ++i)
                coeffs[i] %= Field.Order;

            RemoveLeadingZeros();
        }

        /// <summary>
        /// creates monomial: c * x^deg
        /// </summary>
        /// <param name="field"></param>
        /// <param name="c"></param>
        /// <param name="deg"></param>
        public GFPoly(GF field, ulong c, int deg)
        {
            var coeffs = new ulong[deg + 1];
            coeffs[deg] = c % field.Order;

            Field = field;
            _coeffs = coeffs;

            RemoveLeadingZeros();
        }

        /// <summary>
        /// private default constructor
        /// it's created only for internal methods
        /// in order to set pol GF field and coeffs use the corresponding properties 
        /// </summary>
        private GFPoly() { }

        // [NOTE]:
        // only for internal methods
        // assumes that 'coeff' belongs to [0; field.Order - 1]
        private GFPoly(GF field, ulong coeff)
        {
            Field = field;
            //_coeffs = new ulong[] { coeff % Field.Order };
            _coeffs = new ulong[] { coeff };
        }

        #endregion

        public ulong this[int index]
        {
            get
            {
                if (index < 0 || index >= _coeffs.Length)
                    throw new IndexOutOfRangeException();

                return _coeffs[index];
            }
        }

        #region Misc-Methods

        /// <summary>
        /// converts polynomial to the latex format with the specified variable
        /// </summary>
        /// <param name="var">var of the pol</param>
        /// <returns>latex representation of the polynomial</returns>
        public string ToLatex()
        {
            //if (!char.IsLetter(_var))
            //    throw new("The variable of the polynomial must be a letter of the alphabet: [a-z] or [A-Z]");

            // if the pol is a const
            if (Length == 1)
                return _coeffs[0].ToString();

            // => min polynomial length = 2 => min polynomial form: c_0 + c_1 * x => deg >= 1

            var polLatex = new StringBuilder();
            var polVar = _var.Length == 1 ? _var : @"\" + _var;

            for (int i = Deg; i > 1; --i)
            {
                var coeff = _coeffs[i];

                if (coeff != 0UL)
                {
                    if (polLatex.Length != 0)
                        polLatex.Append('+');

                    polLatex.Append(coeff == 1UL ?
                        "{" + polVar + "}^{" + i.ToString() + "}"
                        : coeff.ToString() + "{" + polVar + "}^{" + i.ToString() + "}");
                }
            }

            if (_coeffs[1] != 0UL)
            {
                if (polLatex.Length != 0)
                    polLatex.Append('+');

                // append c_1 coeff
                polLatex.Append(_coeffs[1] == 1UL ? polVar : _coeffs[1].ToString() + polVar);
            }

            // append '+c_0' coeff if c_0 != 0
            if (_coeffs[0] != 0UL)
                polLatex.Append('+' + _coeffs[0].ToString());

            return polLatex.ToString();
        }

        private void RemoveLeadingZeros()
        {
            int deg = _coeffs.Length - 1;

            for (int i = deg; i > 0; --i)
            {
                if (_coeffs[i] == 0)
                    --deg;
                else
                    break;
            }

            if (deg != _coeffs.Length - 1)
            {
                var newPolCoeffs = new ulong[deg + 1];

                for (int i = 0; i <= deg; ++i)
                    newPolCoeffs[i] = _coeffs[i];

                _coeffs = newPolCoeffs;
            }

            Deg = _coeffs[0] == 0 && deg == 0 ? -1 : deg;
        }

        /// <summary>
        /// creates pol using private-default constructor withour any checks;
        /// note: assumed that the polynomials coeffs are specified correctly
        /// this method allows to create polynomial very efficiently without any checks
        /// </summary>
        /// <param name="field">galois field to which the polynomial belongs</param>
        /// <param name="coeffs">polynomial coefficients</param>
        /// <param name="deg">polynomial degree</param>
        /// <returns>instance of the GFPoly class</returns>
        private static GFPoly CreatePol(GF field, ulong[] coeffs, int deg)
            => new GFPoly()
            {
                Field = field,
                _coeffs = coeffs,
                Deg = deg
            };

        /// <summary>
        /// creates pol using GF: field and ulong[]: coeffs;
        /// note: this method does not reduce the coeffs of the pol;
        /// this method is used under the assumption that
        /// all coeffs of the pol belong to the GF(p) = {0, ..., p - 1};
        /// this method is used internally
        /// </summary>
        /// <param name="field"></param>
        /// <param name="coeffs"></param>
        /// <returns></returns>
        private static GFPoly CreateReducedPol(GF field, ulong[] coeffs)
        {
            var pol = new GFPoly()
            {
                Field = field,
                _coeffs = coeffs
            };

            pol.RemoveLeadingZeros();

            return pol;
        }

        /// <summary>
        /// creates monomial: c * x^deg
        /// [NOTE]:
        /// only for internal methods
        /// assumes that 'c' belongs to [1; field.Order - 1]
        /// </summary>
        /// <param name="field">galois field over which the monomial is considered</param>
        /// <param name="c">monomial coeff</param>
        /// <param name="deg">monomial deg</param>
        /// <returns>monomial: c * x^deg</returns>
        private static GFPoly CreateMonomial(GF field, ulong c, int deg)
        {
            var coeffs = new ulong[deg + 1];
            coeffs[deg] = c;

            return CreatePol(field, coeffs, coeffs[0] == 0 && deg == 0 ? -1 : deg);
        }

        public GFPoly DeepCopy()
        {
            var copyCoeffs = new ulong[_coeffs.Length];

            for (int i = 0; i < copyCoeffs.Length; ++i)
                copyCoeffs[i] = _coeffs[i];

            return CreatePol(Field, copyCoeffs, Deg);
        }

        #region GFp-Arithmetic

        // methods for arithmetric in GF(p)
        // Z_5 = {0, 1, 2, 3, 4}
        // used propety: -x = p - x, x \in GF(p)
        // => a - b = a + p - b = p + a - b:
        // sample: a = 4, b = 3, p = 5 => a - b = 4 - 3 = 4 + 5 - 3 = 4 + 2 = 6 % 5 = 1
        // sample: a = 3, b = 4, p = 5 => a - b = 3 - 4 = 3 + 5 - 4 = 3 + 1 = 4
        // but we can simplify the minus operation for a >= b case, like this:
        // just subtract: a - b
        // [NOTE]: assumes that 'a' and 'b' belongs to [0; p - 1]
        private static ulong SubMod(ulong a, ulong b, ulong p) => a < b ? p + a - b : a - b;

        // [NOTE]: assumes that 'a' belongs to [0; p - 1]
        private static ulong NegateMod(ulong a, ulong p) => a == 0UL ? 0UL : p - a;

        private static ulong InverseMod(GF field, ulong a) => field.Inverse(a);

        /// <summary>
        /// Generates random monic polynomial of degree n over GF(p)
        /// </summary>
        /// <param name="field">galois field GF(p)</param>
        /// <param name="n">degree of the polynomial</param>
        /// <returns>random monic polynomial of degree n over GF(p)</returns>
        public static GFPoly GenerateRandomMonicPol(GF field, int n)
        {
            var rand = new Random();

            var randPolCoeffs = new ulong[n + 1];

            int pInt = (int)field.Order;

            //int rightBound = p < int.MaxValue ? (int)p : int.MaxValue;

            for (int i = 0; i < n; ++i)
                randPolCoeffs[i] = (ulong)rand.Next(0, pInt);

            //Parallel.For(0, n, i => randPolCoeffs[i] = (ulong)rand.Next(0, pInt));

            randPolCoeffs[n] = 1;

            return CreatePol(field, randPolCoeffs, n);
        }

        #endregion

        #endregion

        #region Internal-Methods

        private static GFPoly MulPolByMonom(GFPoly f, int deg)
        {
            int n = f.Length + deg;

            var product = new ulong[n];
            var df = f._coeffs;
            int j = 0;

            for (int i = deg; i < n; ++i)
                product[i] = df[j++];

            return CreatePol(f.Field, product, n - 1);
        }

        #endregion

        // [overloading binary operations]:

        #region Binary-Operations

        #region Arithmetic

        #region GFPoly-GFPoly

        public static GFPoly operator +(GFPoly f, GFPoly g)
        {
            if (f.Field != g.Field)
                throw new Exception($"Cannot add polynomials from different GF({f.Field.Order}) and GF({g.Field.Order}) fields!");

            int df = f._coeffs.Length - 1;
            int dg = g._coeffs.Length - 1;
            int dmax, dmin;

            ulong p = f.Field.Order;

            ulong[] pol1, pol2;

            if (df > dg)
            {
                dmax = df;
                dmin = dg;

                pol1 = f._coeffs;
                pol2 = g._coeffs;
            }
            else
            {
                dmax = dg;
                dmin = df;

                pol1 = g._coeffs;
                pol2 = f._coeffs;
            }

            var resPolCoeffs = new ulong[dmax + 1];

            int i;

            for (i = 0; i <= dmin; ++i)
                resPolCoeffs[i] = (pol1[i] + pol2[i]) % p;

            for (; i <= dmax; ++i)
                resPolCoeffs[i] = pol1[i];

            return CreateReducedPol(f.Field, resPolCoeffs);
        }

        public static GFPoly operator -(GFPoly f, GFPoly g)
        {
            if (f.Field != g.Field)
                throw new Exception($"Cannot subtract polynomials from different GF({f.Field.Order}) and GF({g.Field.Order}) fields!");

            int i;

            ulong p = f.Field.Order;

            ulong[] resPolCoeffs;

            ulong[] fc = f._coeffs;
            ulong[] gc = g._coeffs;

            int df = fc.Length - 1;
            int dg = gc.Length - 1;

            if (df >= dg)
            {
                resPolCoeffs = new ulong[fc.Length];

                for (i = 0; i <= dg; ++i)
                    resPolCoeffs[i] = SubMod(fc[i], gc[i], p);

                for (; i <= df; ++i)
                    resPolCoeffs[i] = fc[i];
            }
            else
            {
                resPolCoeffs = new ulong[gc.Length];

                for (i = 0; i <= df; ++i)
                    resPolCoeffs[i] = SubMod(fc[i], gc[i], p);

                for (; i <= dg; ++i)
                    resPolCoeffs[i] = NegateMod(gc[i], p);
            }

            return CreateReducedPol(f.Field, resPolCoeffs);
        }

        public static GFPoly operator *(GFPoly f, GFPoly g)
        {
            if (f.Field != g.Field)
                throw new Exception($"Cannot multiply polynomials from different GF({f.Field.Order}) and GF({g.Field.Order}) fields!");

            ulong p = f.Field.Order;

            ulong[] fc = f._coeffs;
            ulong[] gc = g._coeffs;

            int df = fc.Length - 1;
            int dg = gc.Length - 1;

            ulong[] resPolCoeffs = new ulong[df + dg + 1];

            //for (int i = 0; i <= dproduct; ++i)
            //    resPolCoeffs[i] = 0;

            for (int i = 0; i <= df; ++i)
                for (int j = 0; j <= dg; ++j)
                    resPolCoeffs[i + j] = (resPolCoeffs[i + j] + fc[i] * gc[j]) % p;

            return CreateReducedPol(f.Field, resPolCoeffs);
        }

        /// <summary>
        /// Returns the quotient of dividing f by g
        /// </summary>
        /// <param name="f"></param>
        /// <param name="g"></param>
        /// <returns>quot(f, g)</returns>
        public static GFPoly operator /(GFPoly f, GFPoly g)
        {
            if (f.Field != g.Field)
                throw new Exception($"Cannot divide polynomials from different GF({f.Field.Order}) and GF({g.Field.Order}) fields!");

            if (g.IsZero)
                throw new DivideByZeroException("Cannot divide polynomial by zero!");

            int df = f._coeffs.Length - 1;
            int dg = g._coeffs.Length - 1;

            if (df < dg)
                return Zero(f.Field);

            ulong p = f.Field.Order;

            ulong[] fc = f._coeffs;
            ulong[] gc = g._coeffs;

            ulong inv = InverseMod(f.Field, gc[dg]);

            var tmp = new ulong[fc.Length];

            if (df >= 0 && dg == 0)
            {
                for (int i = 0; i <= df; ++i)
                    tmp[i] = fc[i] * inv % p;

                return (df == 0) ? new GFPoly(f.Field, tmp[0]) : new GFPoly(f.Field, tmp);
            }

            for (int i = 0; i < fc.Length; ++i)
                tmp[i] = fc[i];

            int dq = df - dg;
            int dr = dg - 1;

            ulong coeff;

            for (int i = df; i >= dg; --i)
            {
                coeff = tmp[i];

                for (int j = Math.Max(0, i - dq); j <= Math.Min(dr, i); ++j)
                    coeff = SubMod(coeff, gc[j] * tmp[i - j + dg] % p, p);

                tmp[i] = coeff * inv % p;
            }

            int index = df - dq;

            while (df > index && tmp[df] == 0UL) df--;

            var quot = new ulong[df - index + 1];

            int k = 0;

            for (int i = index; i <= df; ++i) quot[k++] = tmp[i];

            int degQuot = quot.Length - 1;

            return CreatePol(f.Field, quot, quot[0] == 0 && degQuot == 0 ? -1 : degQuot);
        }

        /// <summary>
        /// Returns the remainder of dividing f by g
        /// </summary>
        /// <param name="f"></param>
        /// <param name="g"></param>
        /// <returns>rem(f, g)</returns>
        public static GFPoly operator %(GFPoly f, GFPoly g)
        {
            if (f.Field != g.Field)
                throw new Exception($"Cannot divide polynomials from different GF({f.Field.Order}) and GF({g.Field.Order}) fields!");

            if (g.IsZero)
                throw new DivideByZeroException("Cannot divide polynomial by zero!");

            int df = f._coeffs.Length - 1;
            int dg = g._coeffs.Length - 1;

            if (df < dg)
                return f.DeepCopy();

            if (df >= 0 && dg == 0)
                return Zero(f.Field);

            ulong p = f.Field.Order;

            ulong[] fc = f._coeffs;
            ulong[] gc = g._coeffs;

            ulong inv = InverseMod(f.Field, gc[dg]);

            ulong coeff;

            var tmp = new ulong[fc.Length];

            for(int i = 0; i < fc.Length; ++i)
                tmp[i] = fc[i];

            int dq = df - dg;
            int dr = dg - 1;

            for (int i = df; i >= 0; --i)
            {
                coeff = tmp[i];

                for (int j = Math.Max(0, i - dq); j <= Math.Min(dr, i); ++j)
                    coeff = SubMod(coeff, gc[j] * tmp[i - j + dg] % p, p);

                if (i >= dg)
                    coeff *= inv;

                tmp[i] = coeff % p;
            }

            while (dr > 0 && tmp[dr] == 0UL) dr--;

            var rem = new ulong[dr + 1];
            int k = 0;

            for (int i = 0; i <= dr; ++i) rem[k++] = tmp[i];

            int degRem = rem.Length - 1;

            return CreatePol(f.Field, rem, rem[0] == 0 && degRem == 0 ? -1 : degRem);
        }

        public static (GFPoly quot, GFPoly rem) QuotRem(GFPoly f, GFPoly g)
        {
            if (f.Field != g.Field)
                throw new Exception($"Cannot divide polynomials from different GF({f.Field.Order}) and GF({g.Field.Order}) fields!");

            if (g.IsZero)
                throw new DivideByZeroException("Cannot divide polynomial by zero!");

            int df = f._coeffs.Length - 1;
            int dg = g._coeffs.Length - 1;

            if (df < dg)
                return (Zero(f.Field), f.DeepCopy());

            ulong p = f.Field.Order;

            ulong[] fc = f._coeffs;
            ulong[] gc = g._coeffs;

            ulong coeff;

            ulong inv = InverseMod(f.Field, gc[dg]);
            var tmp = new ulong[fc.Length];

            if (df >= 0 && dg == 0)
            {
                for (int i = 0; i <= df; ++i)
                    tmp[i] = fc[i] * inv % p;

                return ((df == 0) ? new GFPoly(f.Field, tmp[0]) : new GFPoly(f.Field, tmp), Zero(f.Field));
            }

            for (int i = 0; i < fc.Length; ++i)
                tmp[i] = fc[i];

            int dq = df - dg;
            int dr = dg - 1;

            for (int i = df; i >= 0; --i)
            {
                coeff = tmp[i];

                for (int j = Math.Max(0, i - dq); j <= Math.Min(dr, i); j++)
                    coeff = SubMod(coeff, gc[j] * tmp[i - j + dg] % p, p);

                if (i >= dg)
                    coeff *= inv;

                tmp[i] = coeff % p;
            }

            int index = df - dq;

            while (df > index && tmp[df] == 0UL) df--;

            while (dr > 0 && tmp[dr] == 0UL) dr--;

            var quot = new ulong[df - index + 1];
            var rem = new ulong[dr + 1];

            int k = 0;

            for (int i = index; i <= df; ++i) quot[k++] = tmp[i];

            k = 0;

            for (int i = 0; i <= dr; ++i) rem[k++] = tmp[i];

            int degQuot = quot.Length - 1;
            int degRem = rem.Length - 1;

            return (new GFPoly()
            {
                Field = f.Field,
                _coeffs = quot,
                Deg = quot[0] == 0 && degQuot == 0 ? -1 : degQuot
            },
            new GFPoly()
            {
                Field = f.Field,
                _coeffs = rem,
                Deg = rem[0] == 0 && degRem == 0 ? -1 : degRem
            });
        }

        #endregion

        #region GFPoly-ulong

        public static GFPoly operator +(GFPoly f, ulong c)
        {
            var resPolCoeffs = new ulong[f._coeffs.Length];

            for (int i = 0; i < f.Length; ++i)
                resPolCoeffs[i] = f[i];

            resPolCoeffs[0] = (resPolCoeffs[0] + c) % f.Field.Order;

            int deg = resPolCoeffs.Length - 1;

            return CreatePol(f.Field, resPolCoeffs, deg == 0 && resPolCoeffs[0] == 0 ? -1 : deg);
        }

        public static GFPoly operator -(GFPoly f, ulong c)
        {
            var resPolCoeffs = new ulong[f._coeffs.Length];

            ulong p = f.Field.Order;

            for (int i = 0; i < f.Length; ++i)
                resPolCoeffs[i] = f[i];

            resPolCoeffs[0] = SubMod(resPolCoeffs[0], c % p, p);

            int deg = resPolCoeffs.Length - 1;

            return CreatePol(f.Field, resPolCoeffs, deg == 0 && resPolCoeffs[0] == 0 ? -1 : deg);
        }

        public static GFPoly operator *(GFPoly f, ulong c)
        {
            if (c == 0)
                return Zero(f.Field);

            int df = f.Deg;

            var resPolCoeffs = new ulong[f._coeffs.Length];

            for (int i = 0; i <= df; ++i)
                resPolCoeffs[i] = f[i] * c % f.Field.Order;

            int deg = resPolCoeffs.Length - 1;

            return CreatePol(f.Field, resPolCoeffs, deg == 0 && resPolCoeffs[0] == 0 ? -1 : deg);
        }

        #endregion

        #endregion

        #region Comparison

        public static bool operator ==(GFPoly f, GFPoly g)
        {
            if (f.Field != g.Field)
                throw new Exception($"Cannot compare polynomials from different GF({f.Field.Order}) and GF({g.Field.Order}) fields!");

            int df = f.Deg;
            int dg = g.Deg;

            ulong[] fc = f._coeffs;
            ulong[] gc = g._coeffs;

            if (df != dg)
                return false;

            for (int i = 0; i <= df; ++i)
                if (fc[i] != gc[i])
                    return false;

            return true;
        }

        /// <summary>
        /// Note: if any of the pols (f or g) is equal to null,
        /// then this method throws an exception;
        /// to check the pol for null, use the (null, is not null) keyword
        /// </summary>
        /// <param name="f"></param>
        /// <param name="g"></param>
        /// <returns></returns>
        public static bool operator !=(GFPoly f, GFPoly g) => !(f == g);

        #endregion

        #region Misc

        public static GFPoly GCD(GFPoly f, GFPoly g)
        {
            if (f.Field != g.Field)
                throw new Exception($"Cannot calculate GCD of the polynomials from different GF({f.Field.Order}) and GF({g.Field.Order}) fields!");

            var gf = f.Field;

            // Important: does not work correctly with polynomials of degree zero:
            // for example, gcd(9, 9) mod 19 = 1, should be = 9
            // and also: gcd(9, 0) mod 19 = 1, should be = 9

            int df = f.Deg;
            int dg = g.Deg;

            // if both pols are equal to zero => gcd(0, 0) = 0
            if (df == -1 && dg == -1)
                return Zero(gf);
            else if (df == -1)
                return g.DeepCopy();
            else if (dg == -1)
                return f.DeepCopy();

            GFPoly rem;

            while (g.Deg != -1)
            {
                rem = f % g;
                f = g;
                g = rem;
            }

            // returns: gcd(f(x), g(x)) mod p
            return f * InverseMod(gf, f.LC);
        }

        public static (GFPoly gcd, GFPoly a, GFPoly b) ExtendedGCD(GFPoly f, GFPoly g)
        {
            if (f.Field != g.Field)
                throw new Exception($"Cannot calculate ExtendedGCD of the polynomials from different GF({f.Field.Order}) and GF({g.Field.Order}) fields!");

            GF gf = f.Field;

            if (f.Deg == -1 && g.Deg == -1)
                return (Zero(f.Field), Zero(f.Field), Zero(f.Field));

            // Note: for find: f(x) и g(x) such that: a(x)*f(x) + b(x)*g(x) = gcd(a(x), b(x)) we use recurrent relations:
            // initial conditions:: A_{-1} = 1, B_{-1} = 0, A_0 = 0, B_0 = 1
            // recurrent relations: A_{i} = A_{i-2} - q_{i} * A_{i-1} and B_{i} = B_{i-2} - q_{i} * B_{i-1}

            // Designations:
            // q - quotient
            // r - remainder
            // A - current
            // B - current
            // Ap - A previous: A_{i-1}
            // Bp - B previous: B_{i-1}
            // App - A previous previous: A_{i-2}
            // Bpp - B previous previous: B_{i-2}
            // invLC - inverse leading coefficient: lc^{-1}

            GFPoly Ap, App, Bp, Bpp, q, r, A, B;

            App = One(f.Field);
            Bpp = Zero(f.Field);
            Ap = Bpp;
            Bp = Bpp;

            (GFPoly quot, GFPoly rem) quotRem;

            while (g.Deg != -1)
            {
                quotRem = QuotRem(f, g);

                q = quotRem.quot;
                r = quotRem.rem;

                A = App - q * Ap;
                B = Bpp - q * Bp;

                App = Ap;
                Ap = A;

                Bpp = Bp;
                Bp = B;

                f = g;
                g = r;
            }

            //ulong invLC = InverseMod(gf, f._coeffs[f._coeffs.Length - 1]);
            ulong invLC = InverseMod(gf, f.LC);

            App *= invLC;
            Bpp *= invLC;
            f *= invLC;

            return (f, App, Bpp);
        }

        #endregion

        #endregion

        public static GFPoly Derivative(GFPoly f)
        {
            int df = f._coeffs.Length - 1;

            if (df == 0)
                return Zero(f.Field);

            ulong p = f.Field.Order;

            var fc = f._coeffs;
            var diff = new ulong[df];

            for (ulong i = (ulong)df; i > 0; --i)
                diff[i - 1] = fc[i] * i % p;

            return CreateReducedPol(f.Field, diff);
        }

        public static (ulong lc, GFPoly pol) GetMonicPol(GFPoly f)
        {
            var gf = f.Field;

            ulong lc = f.LC;

            if (f.Deg == -1)
                return (0, One(gf));

            return (lc, lc == 1 ? f.DeepCopy() : f * InverseMod(gf, lc));
        }

        #region Override-Methods

        public override bool Equals(object obj) => obj is GFPoly pol && (this == pol);

        public override int GetHashCode() => HashCode.Combine(_coeffs);

        public override string ToString()
        {
            var coeffsList = "{";

            for (int i = 0; i <= Deg; ++i)
                coeffsList += i != Deg ? $"{_coeffs[i]}," : _coeffs[i].ToString();

            return coeffsList + "}";
        }

        public string ToList()
        {
            var coeffsList = "{";

            for (int i = 0; i < Length; ++i)
                coeffsList += i != Length - 1 ? $"{_coeffs[i]}," : _coeffs[i].ToString();

            return coeffsList + "}";
        }

        #endregion

        // [Methods for factorization]

        #region Square-free-factorization

        #region Misc

        /// <summary>
        /// Raises the pol to the power of 1 / f.Field.Order.
        /// This method only works for special pols that appear in the SFF method
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        private static GFPoly PowPolInverse(GFPoly f)
        {
            var fc = f._coeffs;
            var gf = f.Field;

            int n = fc.Length - 1;

            // [TODO]: fix this because overflow is possible here
            int p = (int)gf.Order;

            int polCoeffsLength = n / p;

            var polCoeffs = new ulong[polCoeffsLength + 1];

            for (int i = 0; i < polCoeffs.Length; ++i)
                polCoeffs[i] = fc[i * p];

            return CreatePol(gf, polCoeffs, polCoeffs[0] == 0 && polCoeffsLength == 0 ? -1 : polCoeffsLength);
        }

        #endregion

        public (ulong lc, List<(GFPoly pol, int exp)> sfFactors) SFF()
        {
            int n = Deg;
            var sfFactors = new List<(GFPoly pol, int exp)>();

            (ulong lc, GFPoly pol) monicPol = GetMonicPol(this);

            if (n <= 1)
                return (monicPol.lc, new List<(GFPoly pol, int exp)>() { (monicPol.pol, 1) });

            var f = monicPol.pol;

            GF gf = f.Field;
            GFPoly c, w, y, z;

            int i = 1;

            c = GCD(f, Derivative(f));
            w = f / c;

            while (!w.IsOne)
            {
                y = GCD(w, c);
                z = w / y;

                if (z.Deg != 0 || z._coeffs[0] != 1)
                    sfFactors.Add((z, i));

                w = y;
                c /= y;
                ++i;
            }

            if (!c.IsOne)
            {
                c = PowPolInverse(c);

                int count = sfFactors.Count;

                sfFactors.AddRange(c.SFF().sfFactors);

                // [TODO]: fix this because overflow is possible
                int p = (int)gf.Order;

                for (int k = count; k < sfFactors.Count; ++k)
                {
                    var t = sfFactors[k];
                    t.exp *= p;
                    sfFactors[k] = t;
                }
            }

            return (monicPol.lc, sfFactors);
        }

        #endregion

        #region Berkelamp-method

        /// <summary>
        /// Creates berlekamp matrix of the form: (B - I)^T
        /// </summary>
        /// <param name="f"></param>
        /// <returns>berlekamp matrix</returns>
        private static ulong[,] CreateBerlekampMatrix(GFPoly f)
        {
            int n = f.Length - 1;
            int k = 0;

            var c = new ulong[n];
            var B = new ulong[n, n];

            ulong[] cTmp;

            var gf = f.Field;

            var fc = f._coeffs;

            var pULong = gf.Order;

            // [TODO]: fix this because overflow is possible
            int pInt = (int)pULong;

            c[0] = 1;

            B[0, 0] = 0;

            ++k;

            int product = (n - 1) * pInt;

            bool flag;

            for (int i = 0; i < product; ++i)
            {
                cTmp = new ulong[n];

                cTmp[0] = NegateMod(c[n - 1], pULong) * fc[0] % pULong;

                flag = ((i + 1) % pInt) == 0;

                for (int j = 1; j < n; ++j)
                {
                    cTmp[j] = SubMod(c[j - 1], c[n - 1] * fc[j] % pULong, pULong);

                    if (flag)
                        B[j, k] = j == k ? SubMod(cTmp[j], 1UL, pULong) : cTmp[j];
                }

                if (flag)
                    B[0, k++] = cTmp[0];

                c = cTmp;
            }

            return B;
        }

        private static List<GFPoly> GetNullSpaceBasis(GF gf, ulong[,] B)
        {
            int i, e;
            bool pivotFound;

            ulong f, a;

            ulong p = gf.Order;
            int n = B.GetLength(0);
            var P = new int[n];
            var S = new List<GFPoly>();

            GFPoly s;

            for (i = 0; i < n; ++i)
                P[i] = -1;

            for (int j = 0; j < n; ++j)
            {
                i = 0;

                pivotFound = false;

                while (!pivotFound && i < n)
                {
                    if (B[i, j] != 0 && P[i] == -1)
                        pivotFound = true;
                    else
                        ++i;
                }

                if (pivotFound)
                {
                    P[i] = j;

                    a = InverseMod(gf, B[i, j]);

                    for (int l = 0; l < n; ++l)
                        B[i, l] = a * B[i, l] % p;

                    for (int k = 0; k < n; ++k)
                    {
                        if (k != i)
                        {
                            f = B[k, j];

                            for (int l = 0; l < n; ++l)
                                B[k, l] = SubMod(B[k, l], f * B[i, l] % p, p);
                        }
                    }
                }
                else
                {
                    s = CreateMonomial(gf, 1, j);

                    for (int l = 0; l < j; ++l)
                    {
                        e = -1;
                        i = 0;

                        while (e == -1 && i < n)
                        {
                            if (l == P[i])
                                e = i;
                            else
                                ++i;
                        }

                        if (e >= 0)
                            s += CreateMonomial(gf, NegateMod(B[e, j], p), l);
                    }

                    S.Add(s);
                }
            }

            return S;
        }

        private static List<GFPoly> FindTrueFactors(GFPoly f, List<GFPoly> basisPols)
        {
            int r = basisPols.Count;

            ulong j;
            ulong p = f.Field.Order;

            GFPoly b, w, g, q;

            var factors = new List<GFPoly>();

            List<GFPoly> oldFactors;

            factors.Add(f);

            for (int k = 1; k < r; ++k)
            {
                b = basisPols[k];

                oldFactors = new List<GFPoly>(factors);

                for (int i = 0; i < oldFactors.Count; ++i)
                {
                    w = oldFactors[i];
                    j = 0;

                    while (j <= p - 1)
                    {
                        g = GCD(b - j, w);

                        if (g.IsOne)
                            j += 1;
                        else if (g == w)
                            j = p;
                        else
                        {
                            factors.Remove(w);

                            q = w / g;

                            factors.Add(g);
                            factors.Add(q);

                            if (factors.Count == r)
                                return factors;
                            else
                            {
                                j += 1;
                                w = q;
                            }
                        }
                    }
                }
            }

            return null;
        }

        #endregion

        #region Cantor-Zassenhaus-method

        #region Misc

        private static GFPoly SqrPolMod(GFPoly f)
        {
            int df = f.Length - 1;
            int dh = 2 * df;

            var fc = f._coeffs;

            var gf = f.Field;
            ulong p = gf.Order;

            var h = new ulong[dh + 1];

            ulong coeff, elem, residue;

            int n, jmin, jmax;

            for (int i = dh; i >= 0; --i)
            {
                coeff = 0;

                jmin = Math.Max(0, i - df);
                jmax = Math.Min(i, df);

                n = jmax - jmin + 1;

                jmax = jmin + (n / 2) - 1;

                for (int j = jmin; j <= jmax; ++j)
                    coeff += fc[j] * fc[i - j] % p;

                coeff += coeff;

                if ((n & 1) == 1)
                {
                    elem = fc[jmax + 1];
                    coeff += elem.ModPow(2, p);
                }

                residue = coeff % p;

                h[i] = residue;
            }

            var resPol = new GFPoly()
            {
                Field = gf,
                _coeffs = h
            };

            resPol.RemoveLeadingZeros();

            return resPol;
        }

        private static GFPoly PowPolOverGF(GFPoly u, int n, GFPoly g, bool isOverGF = false)
        {
            var gf = u.Field;

            var productPol = One(gf);

            if (n == 0)
                return productPol;
            else if (n == 1)
                return isOverGF ? u % g : u.DeepCopy();
            else if (n == 2)
                return isOverGF ? SqrPolMod(u) % g : SqrPolMod(u);
            else
            {
                while (true)
                {
                    if ((n & 1) != 0)
                    {
                        productPol *= u;

                        if (isOverGF)
                            productPol %= g;

                        --n;
                    }

                    n >>= 1;

                    if (n == 0)
                        break;

                    u = SqrPolMod(u);

                    if (isOverGF)
                        u %= g;
                }

                return productPol;
            }
        }

        private static GFPoly PolPowPnSub1Del2OverGF(GFPoly f, int n, GFPoly g, GFPoly[] b)
        {
            // Utility function for Zassenhaus
            // Compute: (f(x))^{(p^n - 1) / 2} mod g(x)

            int pInt = (int)f.Field.Order;

            f %= g;

            GFPoly tmpPol, productPols;

            tmpPol = f;
            productPols = f;

            for (int i = 1; i < n; ++i)
            {
                tmpPol = FrobeniusMap(tmpPol, g, b);

                productPols *= tmpPol;
                productPols %= g;
            }

            return PowPolOverGF(productPols, pInt / 2, g, true);
        }

        /// <summary>
        /// Creates an array with elements: x^{i*p} mod f(x) in GF(p), i = {0, ..., deg f(x) - 1}
        /// </summary>
        /// <param name="f">polynomial f(x) over GF(p)</param>
        /// <returns>array of the form: {x^0, x^1, x^2, ..., x^{(deg f(x) - 1) * p}} mod f(x) in GF(p)</returns>
        private static GFPoly[] FrobeniusMonomialBase(GFPoly f)
        {
            int n = f.Length - 1;

            // [TODO]: throw new Exception("Wrong degree of polynomial - Frobenius monomial base");
            if (n == 0)
                return null;

            var gf = f.Field;

            // [TODO]: fix this because overflow is possible
            int pInt = (int)gf.Order;

            var b = new GFPoly[n];

            b[0] = One(gf);

            if (pInt < n)
            {
                GFPoly monomial;

                for (int i = 1; i < n; ++i)
                {
                    monomial = MulPolByMonom(b[i - 1], pInt);
                    b[i] = monomial % f;
                }
            }
            else if (n > 1)
            {
                b[1] = PowPolOverGF(CreateMonomial(gf, 1, 1),
                                    pInt,
                                    f,
                                    true);

                for (int i = 2; i < n; ++i)
                {
                    b[i] = b[i - 1] * b[1];
                    b[i] %= f;
                }
            }

            return b;
        }

        private static GFPoly FrobeniusMap(GFPoly f, GFPoly g, GFPoly[] b)
        {
            int m = g.Length - 1;

            var fc = f._coeffs;
            var gf = f.Field;

            if (f.Length - 1 >= m)
                f %= g;

            int n = f.Length - 1;

            var resultPol = new GFPoly(gf, fc[0]);

            GFPoly tmp;

            for (int i = 1; i <= n; ++i)
            {
                tmp = b[i] * fc[i];
                resultPol += tmp;
            }

            return resultPol;
        }

        #endregion

        #region Factor

        /// <summary>
        /// Applies an Zassenhaus version of the distinct-degree factorization algorithm to the current polynomial - f(x).
        /// This algorithm splits a square-free polynomial into a product of polynomials whose
        /// irreducible factors all have the same degree.
        /// </summary>
        /// <returns>
        /// the set of all pairs (g(x), d), such that f(x) has an irreducible factor of degree d
        /// and g(x) is the product of all monic irreducible factors of f(x) of degree d
        /// </returns>
        private List<(GFPoly pol, int exp)> DDFZassenhaus()
        {
            // [Note]: f(x) is square-free, monic polynomial over GF(p)[x]

            var f = DeepCopy();
            var gf = f.Field;

            GFPoly monomial, monomialX, g;

            monomialX = CreateMonomial(gf, 1, 1);
            monomial = monomialX;
            var b = FrobeniusMonomialBase(f);

            var factors = new List<(GFPoly, int)>();

            int i = 1;

            while (2 * i <= f.Deg)
            {
                // [Note]: compute (monomial(x))^p mod f(x)
                monomial = FrobeniusMap(monomial, f, b);
                g = GCD(f, monomial - monomialX);

                if (!g.IsOne)
                {
                    factors.Add((g, i));

                    f /= g;
                    monomial %= f;
                    b = FrobeniusMonomialBase(f);
                }

                ++i;
            }

            if (f.Deg > 0 || f._coeffs[0] != 1)
                factors.Add((f, f.Deg));

            return factors;
        }

        /// <summary>
        /// Applies an Zassenhaus version of the equal-degree factorization algorithm to the current polynomial - f(x) with exp: exponent
        /// Note: this is a probabilistic algorithm.
        /// </summary>
        /// <param name="f"></param>
        /// <param name="exponent"></param>
        /// <returns></returns>
        private static List<GFPoly> EDFZassenhaus(GFPoly f, int exponent)
        {
            // Note: f(x) is square-free, monic polynomial over GF(p)[x]

            var factors = new List<GFPoly> { f.DeepCopy() };

            var gf = f.Field;

            ulong p = gf.Order;

            // [TODO]: fix this because overflow is possible
            int pInt = (int)p;

            int n = f.Deg;

            if (f.Deg <= exponent)
                return factors;
            else
            {
                GFPoly[] b = null;

                if (pInt != 2)
                    b = FrobeniusMonomialBase(f);

                n /= exponent;

                GFPoly randPol, oneConstPol, v, g;
                ulong power;

                oneConstPol = One(gf);

                var fieldForRandPol = new GF((ulong)(p < int.MaxValue ? pInt : int.MaxValue), false);

                while (factors.Count < n)
                {
                    // probabilistic part of this algorithm
                    randPol = GenerateRandomMonicPol(fieldForRandPol, 2 * exponent - 1);

                    if (pInt == 2)
                    {
                        v = randPol;

                        power = 2UL.Pow(n * exponent - 1);

                        for (ulong i = 0; i < power; ++i)
                        {
                            randPol = PowPolOverGF(randPol, 2, f, true);
                            v += randPol;
                        }

                        g = GCD(f, v);
                    }
                    else
                    {
                        v = PolPowPnSub1Del2OverGF(randPol, exponent, f, b);
                        g = GCD(f, v - oneConstPol);
                    }

                    if (!g.IsOne && g != f)
                    {
                        var list1 = EDFZassenhaus(g, exponent);
                        var list2 = EDFZassenhaus(f / g, exponent);

                        list1.AddRange(list2);
                        factors = list1;
                    }
                }
            }

            return factors;
        }

        #endregion

        #endregion

        #region Kaltofen-Shoup-method

        /// <summary>
        /// Calculates polynomial composition of the form: g(h(x)) in GF(p)[x]/(f(x))
        /// </summary>
        /// <param name="g"></param>
        /// <param name="h"></param>
        /// <param name="f"></param>
        /// <returns></returns>
        private static GFPoly ComposePolsOverGF(GFPoly g, GFPoly h, GFPoly f)
        {
            int n = g.Length - 1;

            var gf = g.Field;
            var gc = g._coeffs;

            if (n == 0)
                return Zero(gf);

            var composPol = new GFPoly(gf, gc[n]);

            for (int i = n - 1; i >= 0; --i)
            {
                composPol *= h;
                composPol += gc[i];
                composPol %= f;
            }

            return composPol;
        }

        private static (GFPoly, GFPoly) TraceMapPolOverGF(GFPoly a, GFPoly b, GFPoly c, int n, GFPoly f)
        {
            var u = ComposePolsOverGF(a, b, f);
            var v = b;

            GFPoly U, V;

            if ((n & 1) == 1)
            {
                U = a + u;
                V = b;
            }
            else
            {
                U = a;
                V = c;
            }

            n >>= 1;

            while (n != 0)
            {
                u += ComposePolsOverGF(u, v, f);
                v = ComposePolsOverGF(v, v, f);

                if ((n & 1) == 1)
                {
                    U += ComposePolsOverGF(u, V, f);
                    V = ComposePolsOverGF(v, V, f);
                }

                n >>= 1;
            }

            return (ComposePolsOverGF(a, V, f), U);
        }

        private static GFPoly TraceMapPolOverGFShoup(GFPoly f, int n, GFPoly g, GFPoly[] b)
        {
            f %= g;

            var h = f;
            var r = f;

            for (int i = 1; i < n; ++i)
            {
                h = FrobeniusMap(h, g, b);

                r += h;
                r %= g;
            }

            return r;
        }

        #region Factor

        /// <summary>
        /// Applies an Shoup version of the distinct-degree factorization algorithm to the current polynomial - f(x).
        /// This algorithm splits a square-free polynomial into a product of polynomials whose
        /// irreducible factors all have the same degree.
        /// </summary>
        /// <returns>
        /// the set of all pairs (g(x), d), such that f(x) has an irreducible factor of degree d
        /// and g(x) is the product of all monic irreducible factors of f(x) of degree d
        /// </returns>
        private List<(GFPoly pol, int exp)> DDFShoup()
        {
            // [Note]: f(x) is square-free, monic polynomial over GF(p)[x]

            var f = DeepCopy();

            int n = Deg;
            int k = (int)Math.Ceiling(Math.Sqrt(n / 2));

            var monomialX = CreateMonomial(Field, 1, 1);
            var b = FrobeniusMonomialBase(f);

            var h = FrobeniusMap(monomialX, f, b);

            var tmp1 = new GFPoly[k + 1];

            tmp1[0] = monomialX;
            tmp1[1] = h;

            for (int i = 2; i <= k; ++i)
                tmp1[i] = FrobeniusMap(tmp1[i - 1], f, b);

            h = tmp1[k];

            var U = new GFPoly[k];
            var V = new GFPoly[k];

            for (int i = 0; i < k; ++i)
                U[i] = tmp1[i];

            V[0] = h;

            for (int i = 1; i < k; ++i)
                V[i] = ComposePolsOverGF(V[i - 1], h, f);

            var factors = new List<(GFPoly, int)>();

            GFPoly v, g, F;
            int j;

            for (int i = 0; i < V.Length; ++i)
            {
                h = monomialX;
                j = k - 1;

                v = V[i];

                foreach (var u in U)
                {
                    g = v - u;
                    h *= g;
                    h %= f;
                }

                g = GCD(f, h);
                f /= g;

                for (int l = U.Length - 1; l >= 0; --l)
                {
                    h = v - U[l];
                    F = GCD(g, h);

                    if (!F.IsOne)
                        factors.Add((F, k * (i + 1) - j));

                    g /= F;
                    --j;
                }
            }

            if (!f.IsOne)
                factors.Add((f, f.Deg));

            return factors;
        }

        /// <summary>
        /// Applies an Shoup version of the equal-degree factorization algorithm to the current polynomial - f(x) with exp: exponent
        /// Note: this is a probabilistic algorithm.
        /// </summary>
        /// <param name="f"></param>
        /// <param name="exponent"></param>
        /// <returns></returns>
        private static List<GFPoly> EDFShoup(GFPoly f, int exponent)
        {
            int N = f.Length - 1;

            if (N == 0)
                return new List<GFPoly>();

            if (N <= exponent)
                return new List<GFPoly>() { f.DeepCopy() };

            var gf = f.Field;

            // [TODO]: fix this because overflow is possible
            int pInt = (int)gf.Order;

            int q = pInt;

            List<GFPoly> factors;
            var monomialX = CreateMonomial(gf, 1, 1);

            // probabilistic part of this algorithm
            var r = GenerateRandomMonicPol(gf, N - 1);

            GFPoly H, h, h1, h2, h3;

            List<GFPoly> list1, list2, list3;

            if (pInt == 2)
            {
                h = PowPolOverGF(monomialX, q, f, true);
                H = TraceMapPolOverGF(r, h, monomialX, exponent - 1, f).Item2;
                h1 = GCD(f, H);
                h2 = f / h1;

                list1 = EDFShoup(h1, exponent);
                list2 = EDFShoup(h2, exponent);

                list1.AddRange(list2);
                factors = list1;
            }
            else
            {
                var b = FrobeniusMonomialBase(f);
                H = TraceMapPolOverGFShoup(r, exponent, f, b);
                h = PowPolOverGF(H, (q - 1) / 2, f, true);

                h1 = GCD(f, h);
                h2 = GCD(f, h - 1);
                h3 = f / (h1 * h2);

                list1 = EDFShoup(h1, exponent);
                list2 = EDFShoup(h2, exponent);
                list3 = EDFShoup(h3, exponent);

                list2.AddRange(list3);
                list1.AddRange(list2);

                factors = list1;
            }

            return factors;
        }

        #endregion

        #endregion

        #region EDF

        private static List<GFPoly> EDF(FactorMethod method, GFPoly pol, int exp) =>
            method == FactorMethod.CantorZassenhaus ?
                EDFZassenhaus(pol, exp) :
                EDFShoup(pol, exp);

        #endregion

        #region Factorization

        /// <summary>
        /// factors monic square-free polynomial over GF(p)
        /// </summary>
        /// <returns>list of the irreducible factors</returns>
        public List<GFPoly> FactorMSFPol(FactorMethod method = FactorMethod.Auto)
        {
            // [TODO]: add polynomial for checking that is square-free and monic

            // cache galois field
            var gf = Field;

            if (method == FactorMethod.Auto)
                method = FactorMethod.CantorZassenhaus;

            if (method == FactorMethod.Berlekamp)
            {
                // step #1: create berlekamp matrix of the form: (B - E)^T
                var bm = CreateBerlekampMatrix(this);

                // step #2: calculate null space basis of the (B - E)^T matrix
                var basisNSPols = GetNullSpaceBasis(gf, bm);

                // check if the pol is already irreducible
                // if not:
                // step #3: find true factors of the f using basis null space pols
                return (basisNSPols.Count == 1) ?
                       new List<GFPoly>() { DeepCopy() } :
                       FindTrueFactors(this, basisNSPols);
            }

            var DDFFactors = method == FactorMethod.CantorZassenhaus ?
                DDFZassenhaus() :
                DDFShoup();

            var (fPol, fExp) = DDFFactors[0];

            if (DDFFactors.Count == 1)
            {
                // list of the all irreducible factors of the pol f
                var irrFactors = new List<GFPoly>();

                if (fPol.Deg == fExp)
                {
                    // => this pol is already irreducible
                    irrFactors.Add(fPol);
                    return irrFactors;
                }

                var EDFFactors = EDF(method, fPol, fExp);

                for (int j = 0; j < EDFFactors.Count; ++j)
                    irrFactors.Add(EDFFactors[j]);

                return irrFactors;
            }

            // list of the all irreducible factors of the pol f
            var irrFactorsConcurrent = new ConcurrentBag<GFPoly>();

            Parallel.ForEach(DDFFactors, f =>
            {
                var currPol = f.pol;
                var currExp = f.exp;

                if (currPol.Deg == currExp)
                    irrFactorsConcurrent.Add(currPol);
                else
                {
                    var EDFFactors = EDF(method, currPol, currExp);

                    for (int j = 0; j < EDFFactors.Count; ++j)
                        irrFactorsConcurrent.Add(EDFFactors[j]);
                }
            });

            return irrFactorsConcurrent.ToList();
        }

        public (ulong coeff, List<(GFPoly, int)> irrFactors) Factor(FactorMethod method = FactorMethod.Auto, bool forceSort = true)
        {
            var f = this;
            var polVar = _var;

            int n = Deg;

            var monicPol = GetMonicPol(f);

            monicPol.pol._var = polVar;

            var coeff = monicPol.lc;

            if (n <= 1)
                return (coeff, new List<(GFPoly, int)>() { (monicPol.pol, 1) });

            f = monicPol.pol;

            var sfFactors = new List<(GFPoly pol, int exp)>();

            if (!f.IsSquareFree)
            {
                var sfTuple = f.SFF();

                sfFactors.AddRange(sfTuple.sfFactors);
            }
            else
                sfFactors.Add((f, 1));

            // factor comparison rule
            Comparison<(GFPoly pol, int exp)> factorComparator = (f1, f2) =>
            {
                int deg1 = f1.pol.Deg;
                int deg2 = f2.pol.Deg;

                var coeffs1 = f1.pol._coeffs;
                var coeffs2 = f2.pol._coeffs;

                if (deg1 > deg2)
                    return 1;
                else if (deg1 < deg2)
                    return -1;
                else
                {
                    for (int i = 0; i <= deg1; ++i)
                    {
                        if (coeffs1[i] > coeffs2[i])
                            return 1;
                        else if (coeffs1[i] < coeffs2[i])
                            return -1;
                    }

                    return 0;
                }
            };

            var (fPol, fExp) = sfFactors[0];

            if (sfFactors.Count == 1)
            {
                var irrFactors = new List<(GFPoly, int)>();

                fPol._var = polVar;

                // if deg of the pol is equal to 1 => this pol is already irreducible
                if (fPol.Deg == 1)
                {
                    irrFactors.Add((fPol, fExp));

                    return (coeff, irrFactors);
                }

                var currIrrFactors = fPol.FactorMSFPol(method);

                for (int j = 0; j < currIrrFactors.Count; ++j)
                {
                    var currIrrPol = currIrrFactors[j];

                    // set pol var
                    currIrrPol._var = polVar;

                    irrFactors.Add((currIrrPol, fExp));
                }

                // sort irreducible factors, if required
                if (forceSort)
                    irrFactors.Sort(factorComparator);

                return (coeff, irrFactors);
            }

            var irrFactorsConcurrent = new ConcurrentBag<(GFPoly, int)>();

            Parallel.ForEach(sfFactors, sff =>
            {
                var currPol = sff.pol;
                var currPolExp = sff.exp;

                currPol._var = polVar;

                // if deg of the pol is equal to 1 => this pol is already irreducible
                if (currPol.Deg == 1)
                    irrFactorsConcurrent.Add((currPol, currPolExp));
                else
                {
                    var currIrrFactors = currPol.FactorMSFPol(method);

                    for (int j = 0; j < currIrrFactors.Count; ++j)
                    {
                        var currIrrPol = currIrrFactors[j];

                        // set pol var
                        currIrrPol._var = polVar;

                        irrFactorsConcurrent.Add((currIrrPol, currPolExp));
                    }
                }
            });

            // convert irrFactors to List
            var irrFactorsList = irrFactorsConcurrent.ToList();

            // sort irreducible factors, if required
            if (forceSort)
                irrFactorsList.Sort(factorComparator);

            return (coeff, irrFactorsList);
        }

        #endregion

        #region Multithreading-pipeline

        public static List<(ulong coeff, List<(GFPoly, int)> irrFactors)> FactorMultiple(List<GFPoly> pols,
                                                                                         FactorMethod method = FactorMethod.Auto,
                                                                                         bool forceSort = true)
        {
            var factoredPolsDict = new ConcurrentDictionary<long, (ulong coeff, List<(GFPoly, int)> irrFactors)>();

            // factor pols parallel
            var plr = Parallel.ForEach(pols, (pol, _, i) =>
                factoredPolsDict.TryAdd(i, pol.Factor(method, forceSort)));

            // convert dictionary to list:

            var factorList = new List<(ulong coeff, List<(GFPoly, int)> irrFactors)>(factoredPolsDict.Count);

            for (int i = 0; i < factoredPolsDict.Count; ++i)
                factorList.Add(factoredPolsDict[i]);

            return factorList;
        }

        #endregion
    }
}