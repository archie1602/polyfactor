using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace polyfactor.Extensions
{
    public static class UInt64Extension
    {
        /// <summary>
        /// Performs modulus division on a number raised to the power of another number.
        /// </summary>
        /// <param name="value">The number to raise to the exponent power.</param>
        /// <param name="exponent">The exponent to raise value by.</param>
        /// <param name="modulus">The number by which to divide value raised to the exponent power.</param>
        /// <returns>The remainder after dividing valueexponent by modulus.</returns>
        /// Exceptions:
        ///   T:System.DivideByZeroException:
        ///     modulus is zero.
        ///
        ///   T:System.ArgumentOutOfRangeException:
        ///     exponent is negative.
        // [WebSite]: Method 3: https://www.geeksforgeeks.org/multiplicative-inverse-under-modulo-m/
        // Should also consider: [WebSite]: Method 2: https://www.geeksforgeeks.org/modular-exponentiation-power-in-modular-arithmetic/
        public static ulong ModPow(this ulong value, ulong exponent, ulong modulus)
        {
            if (modulus == 0UL)
                throw new DivideByZeroException("Modulus is zero!");

            if (exponent == 0UL)
                return 1UL;

            ulong pow = ModPow(value, exponent / 2UL, modulus) % modulus;
            pow = pow * pow % modulus;

            return (exponent % 2 == 0) ? pow : value * pow % modulus;
        }

        public static ulong Pow(this ulong value, int exponent)
        {
            if (exponent == 0)
                return 1;

            // for an even number, the last bit is zero
            if ((exponent & 1) == 0)
            {
                // shifting one bit to the right is equivalent to dividing by two
                ulong p = Pow(value, exponent >> 1);

                return p * p;
            }
            else
                return value * Pow(value, exponent - 1);
        }

        /// <summary>
        /// [WebSite]: https://jeffhurchalla.com/2018/10/13/implementing-the-extended-euclidean-algorithm-with-unsigned-inputs/
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static (ulong gcd, long x, long y) ExtendedGCD(this ulong a, ulong b)
        {
            long x1 = 1L, y1 = 0L;
            ulong a1 = a;
            long x0 = 0L, y0 = 1L;
            ulong a2 = b, q = 0UL;

            while (a2 != 0UL)
            {
                long x2 = x0 - (long)q * x1;
                long y2 = y0 - (long)q * y1;

                x0 = x1;
                y0 = y1;

                ulong a0 = a1;

                x1 = x2;
                y1 = y2;
                a1 = a2;

                q = a0 / a1;
                a2 = a0 - q * a1;
            }

            return (a1, x1, y1);
        }
    }
}
