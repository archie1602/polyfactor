using System.Collections;
using System.Collections.Generic;

using polyfactor.GaloisStructs;

namespace polyfactor.Test.Data.GFPols.DivPols.Quotient
{
    public class PolsQuotWithDifferentDegsWhereFirstLessShouldReturnPolsQuot : IEnumerable<object[]>
    {
        private readonly GF _gf5 = new GF(5);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[]
            {
                // p1
                new GFPoly(_gf5, new ulong[] { 2, 4, 4 }),
                // p2
                new GFPoly(_gf5, new ulong[] { 1, 0, 3, 4 }),
                // p1 / p2
                new GFPoly(_gf5, new ulong[] { 0 })
            };

            yield return new object[]
            {
                // p1
                new GFPoly(_gf5, new ulong[] { 0, 1, 0, 0, 0 }),
                // p2
                new GFPoly(_gf5, new ulong[] { 0, 0, 4, 5, 6, 108, 0 }),
                // p1 / p2
                new GFPoly(_gf5, new ulong[] { 0 })
            };

            yield return new object[]
            {
                // p1
                new GFPoly(_gf5, new ulong[] { 100, 55, 39, 4, 3, 4, 3, 4 }),
                // p2
                new GFPoly(_gf5, new ulong[] { 4, 1, 2, 3, 4, 0, 0, 0, 0, 0, 0, 99, 859 }),
                // p1 / p2
                new GFPoly(_gf5, new ulong[] { 0 })
            };
        }
    }
}