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
    public class GFTests
    {
        //[Fact]
        //[ExpectedException(typeof(Exception))]
        //public void CreateGFWithOrderZero()
        //{
        //    GF gf0 = new(0);
        //}

        //[Fact]
        //[ExpectedException(typeof(Exception))]
        //public void CreateGFWithOrderOne()
        //{
        //    GF gf1 = new(1);
        //}

        [Fact]
        public void CreateGFWithOrder2()
        {
            try
            {
                GF gf2 = new(2);

                Assert.True(true);
            }
            catch (Exception)
            {
                Assert.True(false);
            }
        }

        [Fact]
        public void CreateGFWithOrder17()
        {
            try
            {
                GF gf17 = new(17);

                Assert.True(true);
            }
            catch (Exception)
            {
                Assert.True(false);
            }
        }

        [Fact]
        public void CreateGFWithOrder55()
        {
            try
            {
                GF gf55 = new(55);

                Assert.True(false);
            }
            catch (Exception)
            {
                Assert.True(true);
            }
        }

        [Fact]
        public void CreateGFWithOrder7919Concurrent()
        {
            try
            {
                GF gf = new(7919);

                for (ulong i = 1; i < gf.Order; ++i)
                    Assert.True(i * gf.Inverse(i) % gf.Order == 1);
            }
            catch (Exception)
            {
                Assert.True(false);
            }
        }
    }
}
