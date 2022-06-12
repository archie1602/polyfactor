using System.Collections;
using System.Collections.Generic;

using polyfactor.GaloisStructs;

namespace polyfactor.Test.Data.GFPols.GCDPols
{
    public class GCDPolsWithEqualDegShouldReturnPolsGCD : IEnumerable<object[]>
    {
        private readonly GF _gf19 = new GF(19);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[]
            {
                // p1
                new GFPoly(_gf19, "9"),
                // p2
                new GFPoly(_gf19, "9"),
                // GCD(p1, p2)
                new GFPoly(_gf19, "9")
            };

            yield return new object[]
            {
                // p1
                new GFPoly(_gf19, "9"),
                // p2
                new GFPoly(_gf19, "0"),
                // GCD(p1, p2)
                new GFPoly(_gf19, "9")
            };

            yield return new object[]
            {
                // p1
                new GFPoly(_gf19, "0"),
                // p2
                new GFPoly(_gf19, "9"),
                // GCD(p1, p2)
                new GFPoly(_gf19, "9")
            };

            yield return new object[]
            {
                // p1
                new GFPoly(_gf19, "0"),
                // p2
                new GFPoly(_gf19, "0"),
                // GCD(p1, p2)
                new GFPoly(_gf19, "0")
            };
        }
    }
}