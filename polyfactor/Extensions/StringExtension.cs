using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.Concurrent;

using polyfactor.GaloisStructs;

namespace polyfactor.Extensions
{
    public static class StringExtension
    {
        /// <summary>
        /// Indicates whether the specified string is a greek letter.
        /// </summary>
        /// <param name="letter">The greek letter to test.</param>
        /// <returns>true if the letter is a greek letter; otherwise, false</returns>
        public static bool IsGreekLetter(this string letter)
        {
            if (string.IsNullOrWhiteSpace(letter))
                return false;

            var greekLetters = new string[]
            {
                "alpha", "A",
                "beta", "B",
                "gamma", "Gamma",
                "delta", "Delta",
                "epsilon", "varepsilon", "E",
                "zeta", "Z",
                "eta", "H",
                "theta", "vartheta", "Theta",
                "iota", "I",
                "kappa", "K",
                "lambda", "Lambda",
                "mu", "M",
                "nu", "N",
                "xi", "Xi",
                "pi", "Pi",
                "rho", "varrho", "P",
                "sigma", "Sigma",
                "tau", "T",
                "upsilon", "Upsilon",
                "phi", "varphi", "Phi",
                "chi", "X",
                "psi", "Psi",
                "omega", "Omega"
            };

            foreach (var gl in greekLetters)
                if (letter == gl)
                    return true;

            return false;
        }

        public static (ulong coeff, List<(GFPoly, int)> irrFactors) Factor(this string polExpr,
                                                                           ulong gfOrder,
                                                                           FactorMethod factorMethod = FactorMethod.Auto,
                                                                           string polVar = null,
                                                                           bool forceGFCheck = true,
                                                                           bool forceFactorsSort = true)
        {
            var pol = new GFPoly(new GF(gfOrder, forceGFCheck), polExpr);

            if (!(polVar is null))
                pol.Var = polVar;

            return pol.Factor(factorMethod, forceFactorsSort);
        }

        public static (ulong coeff, List<(GFPoly, int)> irrFactors) Factor(this string polExpr,
                                                                           ulong gfOrder,
                                                                           string polVar,
                                                                           FactorMethod factorMethod = FactorMethod.Auto,
                                                                           bool forceGFCheck = true,
                                                                           bool forceFactorsSort = true)
        {
            var pol = new GFPoly(new GF(gfOrder, forceGFCheck), polExpr) { Var = polVar };

            return pol.Factor(factorMethod, forceFactorsSort);
        }
    }
}
