using System.Collections;
using System.Collections.Generic;

using polyfactor.GaloisStructs;

namespace polyfactor.Test.Data.GFPols.MulPols
{
    public class MulPolsWithEqualDegShouldMulPols : IEnumerable<object[]>
    {
        private readonly GF _gf5 = new GF(5);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[]
            {
                // p1
                new GFPoly(_gf5, new ulong[] { 1, 5, 569, 2, 1 }),
                // p2
                new GFPoly(_gf5, new ulong[] { 4, 0, 8, 0, 4 }),
                // p1 * p2
                new GFPoly(_gf5, new ulong[] { 4, 0, 4, 3, 0, 1, 4, 3, 4 })
            };

            yield return new object[]
            {
                // p1
                new GFPoly(_gf5, new ulong[] { 0, 6, 4, 3, 1 }),
                // p2
                new GFPoly(_gf5, new ulong[] { 0, 0, 0, 0, 7578 }),
                // p1 * p2
                new GFPoly(_gf5, new ulong[] { 0, 0, 0, 0, 0, 3, 2, 4, 3 })
            };

            yield return new object[]
            {
                // p1
                new GFPoly(_gf5, new ulong[] { 0, 0, 5, 4, 3 }),
                // p2
                new GFPoly(_gf5, new ulong[] { 0, 0, 4, 5, 4 }),
                // p1 * p2
                new GFPoly(_gf5, new ulong[] { 0, 0, 0, 0, 0, 1, 2, 1, 2 })
            };

            yield return new object[]
            {
                // p1
                new GFPoly(_gf5, new ulong[] { 0 }),
                // p2
                new GFPoly(_gf5, new ulong[] { 0 }),
                // p1 * p2
                new GFPoly(_gf5, new ulong[] { 0 })
            };

            yield return new object[]
            {
                // p1
                new GFPoly(_gf5, new ulong[] { 4 }),
                // p2
                new GFPoly(_gf5, new ulong[] { 3 }),
                // p1 * p2
                new GFPoly(_gf5, new ulong[] { 2 })
            };

            yield return new object[]
            {
                // p1
                new GFPoly(_gf5, new ulong[] { 3 }),
                // p2
                new GFPoly(_gf5, new ulong[] { 5 }),
                // p1 * p2
                new GFPoly(_gf5, new ulong[] { 0 })
            };
        }
    }
}