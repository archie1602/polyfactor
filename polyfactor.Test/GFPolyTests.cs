using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Collections;
using System.Text.Encodings.Web;
using System.Collections.Generic;

using Xunit;

using polyfactor.Test.Domain;
using polyfactor.Test.Utils;

using polyfactor.Test.Data.GFPols;
using polyfactor.Test.Data.GFPols.AddPols;
using polyfactor.Test.Data.GFPols.SubPols;
using polyfactor.Test.Data.GFPols.MulPols;
using polyfactor.Test.Data.GFPols.DivPols.Quotient;
using polyfactor.Test.Data.GFPols.DivPols.Remainder;
using polyfactor.Test.Data.GFPols.GCDPols;
using polyfactor.Test.Data.GFPols.ComparePols;
using polyfactor.Test.Data.GFPols.FactorPol;

using polyfactor.GaloisStructs;

namespace polyfactor.Test
{
    public class GFPolyTests
    {
        private GF gf2 = new GF(2);
        private GF gf3 = new GF(3);
        private GF gf5 = new GF(5);
        private GF gf7 = new GF(7);
        private GF gf11 = new GF(11);
        private GF gf19 = new GF(19);
        private GF gf65537 = new GF(65537);

        #region ConvertToLatex


        [Theory]
        [ClassData(typeof(ConvertPolToLatexShouldReturnPolInLatexFormat))]
        public void ConvertPol_ToLatex_ShouldReturnPolInLatexFormat(GFPoly pol, string expectedLatex)
        {
            // ARRANGE

            // ACT
            var actualLatex = pol.ToLatex();

            // ASSERT
            Assert.Equal(expectedLatex, actualLatex);
        }

        #endregion

        #region Equality-Pols

        [Theory]
        [ClassData(typeof(ComparePolsEqualPolsShouldReturnTrue))]
        public void ComparePols_EqualPols_ShouldReturnTrue(GFPoly pol1, GFPoly pol2)
        {
            // ARRANGE

            // ACT
            bool equalityResult = pol1 == pol2;

            // ASSERT
            Assert.True(equalityResult);
        }

        [Theory]
        [ClassData(typeof(ComparePolsNotEqualPolsShouldReturnTrue))]
        public void ComparePols_NotEqualPols_ShouldReturnTrue(GFPoly pol1, GFPoly pol2)
        {
            // ARRANGE

            // ACT
            bool equalityResult = pol1 != pol2;

            // ASSERT
            Assert.True(equalityResult);
        }

        #endregion

        #region Add-Pols

        [Theory]
        [ClassData(typeof(AddPolsWithEqualDegShouldAddPolsData))]
        public void AddPols_WithEqualDeg_ShouldAddPols(GFPoly pol1, GFPoly pol2, GFPoly expectedPol)
        {
            // note: it's case when deg f = deg g (equal)

            // ARRANGE

            // ACT
            var actualPol = pol1 + pol2;

            // ASSERT
            Assert.Equal(expectedPol, actualPol);
        }

        [Theory]
        [ClassData(typeof(AddPolsWithDifferentDegsWhereFirstGreaterShouldAddPols))]
        public void AddPols_WithDifferentDegsWhereFirstGreater_ShouldAddPols(GFPoly pol1, GFPoly pol2, GFPoly expectedPol)
        {
            // note: it's case when deg f > deg g (greater)

            // ARRANGE

            // ACT
            var actualPol = pol1 + pol2;

            // ASSERT
            Assert.Equal(expectedPol, actualPol);
        }

        [Theory]
        [ClassData(typeof(AddPols_WithDifferentDegsWhereFirstLess_ShouldAddPols))]
        public void AddPols_WithDifferentDegsWhereFirstLess_ShouldAddPols(GFPoly pol1, GFPoly pol2, GFPoly expectedPol)
        {
            // note: it's case when deg f < deg g (less)

            // ARRANGE

            // ACT
            var actualPol = pol1 + pol2;

            // ASSERT
            Assert.Equal(expectedPol, actualPol);
        }

        #endregion

        #region Sub-Pols

        [Theory]
        [ClassData(typeof(SubPolsWithEqualDegShouldSubPols))]
        public void SubPols_WithEqualDeg_ShouldSubPols(GFPoly pol1, GFPoly pol2, GFPoly expectedPol)
        {
            // note: it's case when deg f = deg g (equal)

            // ARRANGE

            // ACT
            var actualPol = pol1 - pol2;

            // ASSERT
            Assert.Equal(expectedPol, actualPol);
        }

        [Theory]
        [ClassData(typeof(SubPolsWithDifferentDegsWhereFirstGreaterShouldSubPols))]
        public void SubPols_WithDifferentDegsWhereFirstGreater_ShouldSubPols(GFPoly pol1, GFPoly pol2, GFPoly expectedPol)
        {
            // note: it's case when deg f > deg g (greater)

            // ARRANGE

            // ACT
            var actualPol = pol1 - pol2;

            // ASSERT
            Assert.Equal(expectedPol, actualPol);
        }

        [Theory]
        [ClassData(typeof(SubPolsWithDifferentDegsWhereFirstLessShouldSubPols))]
        public void SubPols_WithDifferentDegsWhereFirstLess_ShouldSubPols(GFPoly pol1, GFPoly pol2, GFPoly expectedPol)
        {
            // note: it's case when deg f < deg g (less)

            // ARRANGE

            // ACT
            var actualPol = pol1 - pol2;

            // ASSERT
            Assert.Equal(expectedPol, actualPol);
        }

        #endregion

        #region Mul-Pols

        [Theory]
        [ClassData(typeof(MulPolsWithEqualDegShouldMulPols))]
        public void MulPols_WithEqualDeg_ShouldMulPols(GFPoly pol1, GFPoly pol2, GFPoly expectedPol)
        {
            // note: it's case when deg f = deg g (equal)

            // ARRANGE

            // ACT
            var actualPol = pol1 * pol2;

            // ASSERT
            Assert.Equal(expectedPol, actualPol);
        }

        [Theory]
        [ClassData(typeof(MulPolsWithDifferentDegsWhereFirstGreaterShouldMulPols))]
        public void MulPols_WithDifferentDegsWhereFirstGreater_ShouldMulPols(GFPoly pol1, GFPoly pol2, GFPoly expectedPol)
        {
            // note: it's case when deg f > deg g (greater)

            // ARRANGE

            // ACT
            var actualPol = pol1 * pol2;

            // ASSERT
            Assert.Equal(expectedPol, actualPol);
        }

        [Theory]
        [ClassData(typeof(MulPolsWithDifferentDegsWhereFirstLessShouldMulPols))]
        public void MulPols_WithDifferentDegsWhereFirstLess_ShouldMulPols(GFPoly pol1, GFPoly pol2, GFPoly expectedPol)
        {
            // note: it's case when deg f < deg g (less)

            // ARRANGE

            // ACT
            var actualPol = pol1 * pol2;

            // ASSERT
            Assert.Equal(expectedPol, actualPol);
        }

        #endregion

        #region Div-Pols

        #region Quotient

        [Theory]
        [ClassData(typeof(PolsQuotWithEqualDegShouldReturnPolsQuot))]
        public void PolsQuot_WithEqualDeg_ShouldReturnPolsQuot(GFPoly pol1, GFPoly pol2, GFPoly expectedPol)
        {
            // note: it's case when deg f = deg g (equal)

            // ARRANGE

            // ACT
            var actualPol = pol1 / pol2;

            // ASSERT
            Assert.Equal(expectedPol, actualPol);
        }

        [Theory]
        [ClassData(typeof(PolsQuotWithDifferentDegsWhereFirstGreaterShouldReturnPolsQuot))]
        public void PolsQuot_WithDifferentDegsWhereFirstGreater_ShouldReturnPolsQuot(GFPoly pol1, GFPoly pol2, GFPoly expectedPol)
        {
            // note: it's case when deg f > deg g (greater)

            // ARRANGE

            // ACT
            var actualPol = pol1 / pol2;

            // ASSERT
            Assert.Equal(expectedPol, actualPol);
        }

        [Theory]
        [ClassData(typeof(PolsQuotWithDifferentDegsWhereFirstLessShouldReturnPolsQuot))]
        public void PolsQuot_WithDifferentDegsWhereFirstLess_ShouldReturnPolsQuot(GFPoly pol1, GFPoly pol2, GFPoly expectedPol)
        {
            // note: it's case when deg f < deg g (less)

            // ARRANGE

            // ACT
            var actualPol = pol1 / pol2;

            // ASSERT
            Assert.Equal(expectedPol, actualPol);
        }

        #endregion

        #region Remainder

        [Theory]
        [ClassData(typeof(PolsRemWithEqualDegShouldReturnPolsRem))]
        public void PolsRem_WithEqualDeg_ShouldReturnPolsRem(GFPoly pol1, GFPoly pol2, GFPoly expectedPol)
        {
            // note: it's case when deg f = deg g (equal)

            // ARRANGE

            // ACT
            var actualPol = pol1 % pol2;

            // ASSERT
            Assert.Equal(expectedPol, actualPol);
        }

        [Theory]
        [ClassData(typeof(PolsRemWithDifferentDegsWhereFirstGreaterShouldReturnPolsRem))]
        public void PolsRem_WithDifferentDegsWhereFirstGreater_ShouldReturnPolsRem(GFPoly pol1, GFPoly pol2, GFPoly expectedPol)
        {
            // note: it's case when deg f > deg g (greater)

            // ARRANGE

            // ACT
            var actualPol = pol1 % pol2;

            // ASSERT
            Assert.Equal(expectedPol, actualPol);
        }

        [Theory]
        [ClassData(typeof(PolsRemWithDifferentDegsWhereFirstLessShouldReturnPolsRem))]
        public void PolsRem_WithDifferentDegsWhereFirstLess_ShouldReturnPolsRem(GFPoly pol1, GFPoly pol2, GFPoly expectedPol)
        {
            // note: it's case when deg f < deg g (less)

            // ARRANGE

            // ACT
            var actualPol = pol1 % pol2;

            // ASSERT
            Assert.Equal(expectedPol, actualPol);
        }

        #endregion

        // [TODO]: add test for QuotRem method

        #endregion

        #region GCD-Pols

        [Theory]
        [ClassData(typeof(GCDPolsWithEqualDegShouldReturnPolsGCD))]
        public void GCDPols_WithEqualDeg_ShouldReturnPolsGCD(GFPoly pol1, GFPoly pol2, GFPoly expectedPol)
        {
            // note: it's case when deg f = deg g (equal)

            // ARRANGE

            // ACT
            var actualPol = GFPoly.GCD(pol1, pol2);

            // ASSERT
            Assert.Equal(expectedPol, actualPol);
        }

        #endregion

        #region Props

        [Fact]
        public void IsPolMonic()
        {
            // init pols
            var pols = new GFPoly[]
            {
                new(gf3, "0"),
                new(gf3, "1"),
                new(gf3, "x"),
                new(gf3, "x^2+2x+2"),
                new(gf3, "2*x^10")
            };

            // expected result
            var expdRes = new bool[]
            {
                false,
                true,
                true,
                true,
                false
            };

            // actual result:
            var actRes = new bool[pols.Length];

            for (int i = 0; i < pols.Length; i++)
                actRes[i] = pols[i].IsMonic;

            // checking:
            for (int i = 0; i < pols.Length; i++)
                Assert.True(expdRes[i] == actRes[i], $"Error occurred at iteration: {i}; pol: {pols[i]}");
        }

        [Fact]
        public void IsPolSquareFree()
        {
            // init pols
            var pols = new GFPoly[]
            {
                new(gf3, "0"),
                new(gf3, "1"),
                new(gf3, "x"),
                new(gf3, "1+2x"),
                new(gf3, "1+2x^2+2x^3+x^5+x^6+2x^8+2x^9+x^11")
            };

            // expected result
            var expdRes = new bool[]
            {
                false,
                true,
                true,
                true,
                false,
            };

            // actual result:
            var actRes = new bool[pols.Length];

            for (int i = 0; i < pols.Length; i++)
                actRes[i] = pols[i].IsSquareFree;

            // checking:
            for (int i = 0; i < pols.Length; i++)
                Assert.True(expdRes[i] == actRes[i], $"Error occurred at iteration: {i}; pol: {pols[i]}");
        }

        #endregion

        #region SFF

        [Fact]
        public void SFFPol()
        {
            // init pols:
            GFPoly f1 = new(gf3, new ulong[] { 1, 0, 2, 2, 0, 1, 1, 0, 2, 2, 0, 1 });
            GFPoly f2 = new(gf7, new ulong[]
            {
                4, 1, 6, 6, 1, 1, 2, 2, 0, 5, 5, 6,
                4, 5, 5, 6, 4, 1, 2, 4, 2, 5, 4, 1,
                2, 2, 5, 3, 4, 6, 2, 5, 0, 2, 1, 2,
                5, 2, 4, 4, 0, 4, 4, 5, 0, 4, 6, 2, 5, 4
            });
            GFPoly f3 = new(gf11, new ulong[] { 1, 5, 6, 3, 5, 7, 10, 6, 7, 4, 5 });

            // expected factorization lists:
            var efl1 = (1UL, new List<(GFPoly, int)>()
            {
                (new(gf3, new ulong[] { 1, 1 }), 1),
                (new(gf3, new ulong[] { 1, 0, 1 }), 3),
                (new(gf3, new ulong[] { 2, 1 }), 4),
            });

            var efl2 = (4UL, new List<(GFPoly, int)>()
            {
                (new(gf7, new ulong[] { 3, 2, 1 }), 5),
                (new(gf7, new ulong[] { 5, 5, 4, 1 }), 2),
                (new(gf7, new ulong[] { 6, 5, 2, 1, 1, 6, 1, 1, 6, 1, 2, 1 }), 3),
            });

            var efl3 = (5UL, new List<(GFPoly, int)>()
            {
                (new(gf11, new ulong[] { 9, 1, 10, 5, 1, 8, 2, 10, 8, 3, 1 }), 1),
            });

            // actual factorization lists
            (ulong, List<(GFPoly pol, int exp)>) afl1, afl2, afl3;

            afl1 = f1.SFF();
            afl2 = f2.SFF();
            afl3 = f3.SFF();

            // compare lambdas expression
            Func<(ulong, ulong),
                 (List<(GFPoly, int)>, List<(GFPoly, int)>),
                 bool> pred = (t1, t2) => t1.Item1 == t1.Item2 &&
                                          t2.Item1.Count == t2.Item2.Count &&
                                          Enumerable.All(t2.Item1, t2.Item2.Contains);

            Assert.True(pred(
                               (efl1.Item1, afl1.Item1),
                               (efl1.Item2, afl1.Item2)
                               ));

            Assert.True(pred(
                               (efl2.Item1, afl2.Item1),
                               (efl2.Item2, afl2.Item2)
                               ));

            Assert.True(pred(
                   (efl3.Item1, afl3.Item1),
                   (efl3.Item2, afl3.Item2)
                   ));
        }

        #endregion

        #region Factorization

        [Theory]
        [ClassData(typeof(FactorPolWithBerlekampMethodShouldReturnRightFactorization))]
        public void FactorPol_WithBerlekampMethod_ShouldReturnRightFactorization(GFPoly pol, (ulong coeff, List<(GFPoly, int)> irrFactors) expectedFactors)
        {
            // ARRANGE

            Func<(ulong, ulong),
                 (List<(GFPoly, int)>, List<(GFPoly, int)>),
                 bool> comparator = (t1, t2) => t1.Item1 == t1.Item2 &&
                                          t2.Item1.Count == t2.Item2.Count &&
                                          Enumerable.All(t2.Item1, t2.Item2.Contains);

            // ACT
            var actualFactors = pol.Factor(FactorMethod.Berlekamp);

            // ASSERT
            Assert.True(comparator((expectedFactors.coeff, actualFactors.coeff), (expectedFactors.irrFactors, actualFactors.irrFactors)));
        }

        [Theory]
        [ClassData(typeof(FactorPolWithCantorZassenhausMethodShouldReturnRightFactorization))]
        public void FactorPol_WithCantorZassenhausMethod_ShouldReturnRightFactorization(GFPoly pol, (ulong coeff, List<(GFPoly, int)> irrFactors) expectedFactors)
        {
            // ARRANGE

            Func<(ulong, ulong),
                 (List<(GFPoly, int)>, List<(GFPoly, int)>),
                 bool> comparator = (t1, t2) => t1.Item1 == t1.Item2 &&
                                          t2.Item1.Count == t2.Item2.Count &&
                                          Enumerable.All(t2.Item1, t2.Item2.Contains);

            // ACT
            var actualFactors = pol.Factor(FactorMethod.CantorZassenhaus);

            // ASSERT
            Assert.True(comparator((expectedFactors.coeff, actualFactors.coeff), (expectedFactors.irrFactors, actualFactors.irrFactors)));
        }

        [Theory]
        [ClassData(typeof(FactorPolWithKaltofenShoupMethodShouldReturnRightFactorization))]
        public void FactorPol_WithKaltofenShoupMethod_ShouldReturnRightFactorization(GFPoly pol, (ulong coeff, List<(GFPoly, int)> irrFactors) expectedFactors)
        {
            // ARRANGE

            Func<(ulong, ulong),
                 (List<(GFPoly, int)>, List<(GFPoly, int)>),
                 bool> comparator = (t1, t2) => t1.Item1 == t1.Item2 &&
                                          t2.Item1.Count == t2.Item2.Count &&
                                          Enumerable.All(t2.Item1, t2.Item2.Contains);

            // ACT
            var actualFactors = pol.Factor(FactorMethod.KaltofenShoup);

            // ASSERT
            Assert.True(comparator((expectedFactors.coeff, actualFactors.coeff), (expectedFactors.irrFactors, actualFactors.irrFactors)));
        }

        [Fact]
        public void FactorBerlekmapTesting()
        {
            GFPoly f1 = new(gf3, new ulong[] { 0, 1, 1, 1, 2, 0, 1 });

            var res = f1.Factor(FactorMethod.Berlekamp);
        }

        [Fact]
        public void FactorBerlekmapTest()
        {
            // init pols:
            GFPoly f1 = new(gf2, new ulong[] { 1, 1, 1, 0, 1 });
            GFPoly f2 = new(gf5, new ulong[] { 1, 4, 1, 1, 1, 1 });
            GFPoly f3 = new(gf2, new ulong[] { 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 });
            GFPoly f4 = new(gf65537, new ulong[]
            {
                0, 3701, 62574, 27537, 12544, 8584, 27177, 4722, 32405, 40930, 55163, 59933, 47217,
                33965, 32427, 10894, 3733, 5974, 38872, 5926, 2135, 45189, 21618, 41507, 35456,
                21757, 20160, 39, 37969, 4211, 59842, 43259, 52677, 143, 62657, 16092, 25280, 45462,
                8, 8, 8, 1
            });

            // expected irreducible factors:

            var expecIrrFactors1 = new List<GFPoly>()
            {
                new(gf2, new ulong[] { 1, 1 }),
                new(gf2, new ulong[] { 1, 0, 1, 1 })
            };

            var expecIrrFactors2 = new List<GFPoly>()
            {
                new(gf5, new ulong[] { 3, 1, 2, 1 }),
                new(gf5, new ulong[] { 2, 4, 1 })
            };

            var expecIrrFactors3 = new List<GFPoly>()
            {
                new(gf2, new ulong[] { 1, 1, 1 }),
                new(gf2, new ulong[] { 1, 0, 1, 1, 0, 1, 1, 0, 1, 1 })
            };

            var expecIrrFactors4 = new List<GFPoly>()
            {
                new(gf65537, new ulong[] { 0, 1 }),
                new(gf65537, new ulong[] { 7, 1 }),
                new(gf65537, new ulong[] { 256, 1 }),
                new(gf65537, new ulong[] { 1593, 1 }),
                new(gf65537, new ulong[] { 28404, 1 }),
                new(gf65537, new ulong[] { 52907, 1 }),
                new(gf65537, new ulong[] { 57761, 1 }),
                new(gf65537, new ulong[] { 65281, 1 }),
                new(gf65537, new ulong[] { 65326, 1 }),
                new(gf65537, new ulong[] { 1068, 22093, 6183, 1 }),
                new(gf65537, new ulong[] { 7, 1, 0, 0, 0, 1 }),
                new(gf65537, new ulong[] { 36744, 13504, 41478, 60957, 17165, 1 }),
                new(gf65537, new ulong[] { 23122, 57915, 31304, 63196, 16370, 37688, 33606, 49763, 1 }),
                new(gf65537, new ulong[] { 26154, 52299, 47738, 47879, 57020, 20990, 46702, 47254, 36784, 63277, 48584, 1 }),
            };

            // actual irreducible factors:
            List<GFPoly> actIrrFactors1, actIrrFactors2, actIrrFactors3, actIrrFactors4;

            actIrrFactors1 = f1.FactorMSFPol(FactorMethod.Berlekamp);
            actIrrFactors2 = f2.FactorMSFPol(FactorMethod.Berlekamp);
            actIrrFactors3 = f3.FactorMSFPol(FactorMethod.Berlekamp);
            actIrrFactors4 = f4.FactorMSFPol(FactorMethod.Berlekamp);

            // compare lambdas expression
            Func<List<GFPoly>, List<GFPoly>, bool> pred = (t1, t2) =>
                t1.Count == t2.Count &&
                Enumerable.All(t1, t2.Contains);

            Assert.True(pred(expecIrrFactors1, actIrrFactors1));
            Assert.True(pred(expecIrrFactors2, actIrrFactors2));
            Assert.True(pred(expecIrrFactors3, actIrrFactors3));
            Assert.True(pred(expecIrrFactors4, actIrrFactors4));
        }

        [Fact]
        public void FactorCantorZassenhausTest()
        {
            // init pols:
            GFPoly f1 = new(gf2, new ulong[] { 1, 1, 1, 0, 1 });
            GFPoly f2 = new(gf5, new ulong[] { 1, 4, 1, 1, 1, 1 });
            GFPoly f3 = new(gf2, new ulong[] { 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 });
            GFPoly f4 = new(gf65537, new ulong[]
            {
                0, 3701, 62574, 27537, 12544, 8584, 27177, 4722, 32405, 40930, 55163, 59933, 47217,
                33965, 32427, 10894, 3733, 5974, 38872, 5926, 2135, 45189, 21618, 41507, 35456,
                21757, 20160, 39, 37969, 4211, 59842, 43259, 52677, 143, 62657, 16092, 25280, 45462,
                8, 8, 8, 1
            });

            // expected irreducible factors:

            var expecIrrFactors1 = new List<GFPoly>()
            {
                new(gf2, new ulong[] { 1, 1 }),
                new(gf2, new ulong[] { 1, 0, 1, 1 })
            };

            var expecIrrFactors2 = new List<GFPoly>()
            {
                new(gf5, new ulong[] { 3, 1, 2, 1 }),
                new(gf5, new ulong[] { 2, 4, 1 })
            };

            var expecIrrFactors3 = new List<GFPoly>()
            {
                new(gf2, new ulong[] { 1, 1, 1 }),
                new(gf2, new ulong[] { 1, 0, 1, 1, 0, 1, 1, 0, 1, 1 })
            };

            var expecIrrFactors4 = new List<GFPoly>()
            {
                new(gf65537, new ulong[] { 0, 1 }),
                new(gf65537, new ulong[] { 7, 1 }),
                new(gf65537, new ulong[] { 256, 1 }),
                new(gf65537, new ulong[] { 1593, 1 }),
                new(gf65537, new ulong[] { 28404, 1 }),
                new(gf65537, new ulong[] { 52907, 1 }),
                new(gf65537, new ulong[] { 57761, 1 }),
                new(gf65537, new ulong[] { 65281, 1 }),
                new(gf65537, new ulong[] { 65326, 1 }),
                new(gf65537, new ulong[] { 1068, 22093, 6183, 1 }),
                new(gf65537, new ulong[] { 7, 1, 0, 0, 0, 1 }),
                new(gf65537, new ulong[] { 36744, 13504, 41478, 60957, 17165, 1 }),
                new(gf65537, new ulong[] { 23122, 57915, 31304, 63196, 16370, 37688, 33606, 49763, 1 }),
                new(gf65537, new ulong[] { 26154, 52299, 47738, 47879, 57020, 20990, 46702, 47254, 36784, 63277, 48584, 1 }),
            };

            // actual irreducible factors:
            List<GFPoly> actIrrFactors1, actIrrFactors2, actIrrFactors3, actIrrFactors4;

            actIrrFactors1 = f1.FactorMSFPol(FactorMethod.CantorZassenhaus);
            actIrrFactors2 = f2.FactorMSFPol(FactorMethod.CantorZassenhaus);
            actIrrFactors3 = f3.FactorMSFPol(FactorMethod.CantorZassenhaus);
            actIrrFactors4 = f4.FactorMSFPol(FactorMethod.CantorZassenhaus);

            // compare lambdas expression
            Func<List<GFPoly>, List<GFPoly>, bool> pred = (t1, t2) =>
                t1.Count == t2.Count &&
                Enumerable.All(t1, t2.Contains);

            Assert.True(pred(expecIrrFactors1, actIrrFactors1));
            Assert.True(pred(expecIrrFactors2, actIrrFactors2));
            Assert.True(pred(expecIrrFactors3, actIrrFactors3));
            Assert.True(pred(expecIrrFactors4, actIrrFactors4));
        }

        [Fact]
        public void FactorKaltofenShoupTest()
        {
            // init pols:
            GFPoly f1 = new(gf2, new ulong[] { 1, 1, 1, 0, 1 });
            GFPoly f2 = new(gf5, new ulong[] { 1, 4, 1, 1, 1, 1 });
            GFPoly f3 = new(gf2, new ulong[] { 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 });
            GFPoly f4 = new(gf65537, new ulong[]
            {
                0, 3701, 62574, 27537, 12544, 8584, 27177, 4722, 32405, 40930, 55163, 59933, 47217,
                33965, 32427, 10894, 3733, 5974, 38872, 5926, 2135, 45189, 21618, 41507, 35456,
                21757, 20160, 39, 37969, 4211, 59842, 43259, 52677, 143, 62657, 16092, 25280, 45462,
                8, 8, 8, 1
            });

            // expected irreducible factors:

            var expecIrrFactors1 = new List<GFPoly>()
            {
                new(gf2, new ulong[] { 1, 1 }),
                new(gf2, new ulong[] { 1, 0, 1, 1 })
            };

            var expecIrrFactors2 = new List<GFPoly>()
            {
                new(gf5, new ulong[] { 3, 1, 2, 1 }),
                new(gf5, new ulong[] { 2, 4, 1 })
            };

            var expecIrrFactors3 = new List<GFPoly>()
            {
                new(gf2, new ulong[] { 1, 1, 1 }),
                new(gf2, new ulong[] { 1, 0, 1, 1, 0, 1, 1, 0, 1, 1 })
            };

            var expecIrrFactors4 = new List<GFPoly>()
            {
                new(gf65537, new ulong[] { 0, 1 }),
                new(gf65537, new ulong[] { 7, 1 }),
                new(gf65537, new ulong[] { 256, 1 }),
                new(gf65537, new ulong[] { 1593, 1 }),
                new(gf65537, new ulong[] { 28404, 1 }),
                new(gf65537, new ulong[] { 52907, 1 }),
                new(gf65537, new ulong[] { 57761, 1 }),
                new(gf65537, new ulong[] { 65281, 1 }),
                new(gf65537, new ulong[] { 65326, 1 }),
                new(gf65537, new ulong[] { 1068, 22093, 6183, 1 }),
                new(gf65537, new ulong[] { 7, 1, 0, 0, 0, 1 }),
                new(gf65537, new ulong[] { 36744, 13504, 41478, 60957, 17165, 1 }),
                new(gf65537, new ulong[] { 23122, 57915, 31304, 63196, 16370, 37688, 33606, 49763, 1 }),
                new(gf65537, new ulong[] { 26154, 52299, 47738, 47879, 57020, 20990, 46702, 47254, 36784, 63277, 48584, 1 }),
            };

            // actual irreducible factors:
            List<GFPoly> actIrrFactors1, actIrrFactors2, actIrrFactors3, actIrrFactors4;

            actIrrFactors1 = f1.FactorMSFPol(FactorMethod.KaltofenShoup);
            actIrrFactors2 = f2.FactorMSFPol(FactorMethod.KaltofenShoup);
            actIrrFactors3 = f3.FactorMSFPol(FactorMethod.KaltofenShoup);
            actIrrFactors4 = f4.FactorMSFPol(FactorMethod.KaltofenShoup);

            // compare lambdas expression
            Func<List<GFPoly>, List<GFPoly>, bool> pred = (t1, t2) =>
                t1.Count == t2.Count &&
                Enumerable.All(t1, t2.Contains);

            Assert.True(pred(expecIrrFactors1, actIrrFactors1));
            Assert.True(pred(expecIrrFactors2, actIrrFactors2));
            Assert.True(pred(expecIrrFactors3, actIrrFactors3));
            Assert.True(pred(expecIrrFactors4, actIrrFactors4));
        }

        #endregion
    }
}
