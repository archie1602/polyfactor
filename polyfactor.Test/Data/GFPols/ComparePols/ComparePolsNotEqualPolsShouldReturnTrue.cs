using System.Collections;
using System.Collections.Generic;

using polyfactor.GaloisStructs;

namespace polyfactor.Test.Data.GFPols.ComparePols
{
    public class ComparePolsNotEqualPolsShouldReturnTrue : IEnumerable<object[]>
    {
        private readonly GF _gf5 = new GF(5);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[]
            {
                new GFPoly(_gf5, new ulong[] { 1, 3, 3, 2, 1 }),
                new GFPoly(_gf5, new ulong[] { 4, 0, 2, 3, 4 })
            };

            yield return new object[]
            {
                new GFPoly(_gf5, new ulong[] { 999, 3, 3, 2, 1 }),
                new GFPoly(_gf5, new ulong[] { 4 })
            };

            yield return new object[]
            {
                new GFPoly(_gf5, new ulong[] { 0 }),
                new GFPoly(_gf5, new ulong[] { 758 })
            };
        }
    }
}