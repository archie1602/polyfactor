using System.Collections;
using System.Collections.Generic;

using polyfactor.GaloisStructs;

namespace polyfactor.Test.Data.GFPols
{
    public class ConvertPolToLatexShouldReturnPolInLatexFormat : IEnumerable<object[]>
    {
        private readonly GF _gf7 = new GF(7);
        private readonly char _polVar = 'x';

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[]
            {
                new GFPoly(_gf7, new ulong[] { 0 }),
                "0"
            };

            yield return new object[]
            {
                new GFPoly(_gf7, new ulong[] { 1 }),
                "1"
            };

            yield return new object[]
            {
                new GFPoly(_gf7, new ulong[] { 1, 1 }),
                $"{_polVar}+1"
            };

            yield return new object[]
            {
                new GFPoly(_gf7, new ulong[] { 0, 1 }),
                $"{_polVar}"
            };

            yield return new object[]
            {
                new GFPoly(_gf7, new ulong[] { 0, 4 }),
                $"4{_polVar}"
            };

            yield return new object[]
            {
                new GFPoly(_gf7, new ulong[] { 0, 0 }),
                "0"
            };

            yield return new object[]
            {
                new GFPoly(_gf7, new ulong[] { 0, 0, 0, 3, 0, 2, 3, 0, 1 }),
                $"{{{_polVar}}}^{{8}}+3{{{_polVar}}}^{{6}}+2{{{_polVar}}}^{{5}}+3{{{_polVar}}}^{{3}}"
            };

            yield return new object[]
            {
                new GFPoly(_gf7, new ulong[] { 0, 0, 0, 3, 0, 2, 3, 4, 5 }),
                $"5{{{_polVar}}}^{{8}}+4{{{_polVar}}}^{{7}}+3{{{_polVar}}}^{{6}}+2{{{_polVar}}}^{{5}}+3{{{_polVar}}}^{{3}}"
            };

            yield return new object[]
            {
                new GFPoly(_gf7, new ulong[] { 0, 0, 0, 0, 0, 0, 0, 0, 1 }),
                $"{{{_polVar}}}^{{8}}"
            };

            yield return new object[]
            {
                new GFPoly(_gf7, new ulong[] { 0, 0, 0, 0, 0, 0, 0, 0, 5 }),
                $"5{{{_polVar}}}^{{8}}"
            };

            yield return new object[]
            {
                new GFPoly(_gf7, new ulong[] { 0, 0, 0, 0, 0, 0, 0, 0, 0 }),
                "0"
            };

            yield return new object[]
            {
                new GFPoly(_gf7, new ulong[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 }),
                $"{{{_polVar}}}^{{10}}"
            };

            yield return new object[]
            {
                new GFPoly(_gf7, new ulong[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 4 }),
                $"4{{{_polVar}}}^{{10}}"
            };

            yield return new object[]
            {
                new GFPoly(_gf7, new ulong[] { 0, 0, 0, 5, 0, 0, 0, 0, 0, 0, 6, 4 }),
                $"4{{{_polVar}}}^{{11}}+6{{{_polVar}}}^{{10}}+5{{{_polVar}}}^{{3}}"
            };

            yield return new object[]
            {
                new GFPoly(_gf7, new ulong[] { 0, 0, 0, 5, 0, 0, 0, 0, 0, 0, 6, 4, 7, 7, 8, 7 }),
                $"{{{_polVar}}}^{{14}}+4{{{_polVar}}}^{{11}}+6{{{_polVar}}}^{{10}}+5{{{_polVar}}}^{{3}}"
            };
        }
    }
}