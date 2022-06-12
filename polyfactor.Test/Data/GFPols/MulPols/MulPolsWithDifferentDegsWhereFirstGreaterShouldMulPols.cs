using System.Collections;
using System.Collections.Generic;

using polyfactor.GaloisStructs;

namespace polyfactor.Test.Data.GFPols.MulPols
{
    public class MulPolsWithDifferentDegsWhereFirstGreaterShouldMulPols : IEnumerable<object[]>
    {
        private readonly GF _gf5 = new GF(5);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[]
            {
                // p1
                new GFPoly(_gf5, new ulong[] { 4, 5, 3 }),
                // p2
                new GFPoly(_gf5, new ulong[] { 5, 4 }),
                // p1 * p2
                new GFPoly(_gf5, new ulong[] { 0, 1, 0, 2 })
            };

            yield return new object[]
            {
                // p1
                new GFPoly(_gf5, new ulong[] { 0, 0, 1, 4, 3 }),
                // p2
                new GFPoly(_gf5, new ulong[] { 10, 5, 4 }),
                // p1 * p2
                new GFPoly(_gf5, new ulong[] { 0, 0, 0, 0, 4, 1, 2 })
            };

            yield return new object[]
            {
                // p1
                new GFPoly(_gf5, new ulong[] { 0, 4, 1, 5, 3, 4, 3, 4 }),
                // p2
                new GFPoly(_gf5, new ulong[] { 5, 1, 0, 29, 55 }),
                // p1 * p2
                new GFPoly(_gf5, new ulong[] { 0, 0, 4, 1, 1, 2, 4, 0, 0, 2, 1 })
            };
        }
    }
}