using System.Collections;
using System.Collections.Generic;

using polyfactor.GaloisStructs;

namespace polyfactor.Test.Data.GFPols.SubPols
{
    public class SubPolsWithDifferentDegsWhereFirstGreaterShouldSubPols : IEnumerable<object[]>
    {
        private readonly GF _gf5 = new GF(5);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[]
            {
                // p1
                new GFPoly(_gf5, new ulong[] { 1, 2, 4, 3 }),
                // p2
                new GFPoly(_gf5, new ulong[] { 1, 2, 3 }),
                // p1 - p2
                new GFPoly(_gf5, new ulong[] { 0, 0, 1, 3 })
            };

            yield return new object[]
            {
                // p1
                new GFPoly(_gf5, new ulong[] { 0, 0, 1, 4, 3 }),
                // p2
                new GFPoly(_gf5, new ulong[] { 0, 0, 4 }),
                // p1 - p2
                new GFPoly(_gf5, new ulong[] { 0, 0, 2, 4, 3 })
            };

            yield return new object[]
            {
                // p1
                new GFPoly(_gf5, new ulong[] { 4, 4, 1, 4, 3, 4, 3, 4 }),
                // p2
                new GFPoly(_gf5, new ulong[] { 4, 1, 2, 3, 4 }),
                // p1 - p2
                new GFPoly(_gf5, new ulong[] { 0, 3, 4, 1, 4, 4, 3, 4 })
            };
        }
    }
}
