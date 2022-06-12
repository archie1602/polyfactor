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
    public class FactorValidCheck
    {
        private static (bool flag, ulong[] coeffs) GetNextPol(ulong[] arr, ulong p)
        {
            var coeffs = new ulong[arr.Length];

            // copy 'arr' to 'coeffs'
            for (int i = 0; i < arr.Length; i++)
                coeffs[i] = arr[i];

            for (int i = 0; i < arr.Length; ++i)
            {
                if (coeffs[i] == p - 1)
                    coeffs[i] = 0;
                else
                {
                    coeffs[i]++;
                    return (true, coeffs);
                }
            }

            return (false, coeffs);
        }

        private static List<GFPoly> GeneratePolsOverGF(GF gf, int deg, int numPols, bool IsFromZero = true)
        {
            var init = new ulong[deg + 1];

            if (!IsFromZero)
                init[^1] = 1;

            ulong p = gf.Order;

            (bool flag, ulong[] coeffs) t = (true, init);

            var pols = new List<GFPoly>();

            int i = 0;

            while (t.flag && i < numPols)
            {
                var pol = new GFPoly(gf, t.coeffs);

                pols.Add(pol);

                t = GetNextPol(t.coeffs, p);
                i++;
            }

            return pols;
        }

        private static (ulong coeff, List<(GFPoly, int)> irrFactors) ConvertFactorFromJsonToGFPoly(List<FactorPart> factors, GF gf)
        {
            var irrFactors = new List<(GFPoly, int)>();
            ulong coeff = ulong.Parse(factors[0].pol);

            if (factors.Count == 1)
                irrFactors.Add((new(gf, "1"), 1));
            else
            {
                for (int i = 1; i < factors.Count; i++)
                {
                    var currFactor = factors[i];

                    var currPolStr = currFactor.pol;
                    var curreExpStr = currFactor.exp;

                    var pol = new GFPoly(gf, currPolStr);

                    irrFactors.Add((pol, int.Parse(curreExpStr)));
                }
            }

            return (coeff, irrFactors);
        }

        private static (bool flag, ulong[] coeffs) GetNextPol(ulong[] arr, ulong p, int lastDeg)
        {
            var coeffs = new ulong[arr.Length];

            // copy 'arr' to 'coeffs'
            for (int i = 0; i < arr.Length; i++)
                coeffs[i] = arr[i];

            for (int i = 0; i <= lastDeg; ++i)
            {
                if (coeffs[i] == p - 1)
                    coeffs[i] = 0;
                else
                {
                    coeffs[i]++;
                    return (true, coeffs);
                }
            }

            return (false, coeffs);
        }

        private static List<GFPoly> GenerateMSFPolsOverGF(GF gf, int deg, int numPols)
        {
            var init = new ulong[deg + 1];
            init[^1] = 1;

            var p = gf.Order;

            (bool flag, ulong[] coeffs) t = (true, init);

            var pols = new List<GFPoly>();

            int i = 0;

            while (t.flag && i < numPols)
            {
                var pol = new GFPoly(gf, t.coeffs);

                if (pol.IsSquareFree)
                {
                    pols.Add(pol);
                    i++;
                }

                t = GetNextPol(t.coeffs, p, deg - 1);
            }

            return pols;
        }

        private static (List<(ulong, List<(GFPoly, int)>)>, List<(ulong, List<(GFPoly, int)>)>) CheckValid(GF gf, int deg, int numPols, FactorMethod method)
        {
            // prepare

            //var gf = new GF(3);
            //int deg = 5;
            //int numPols = (int)Math.Pow(gf.Order, deg + 1);
            //var method = FactorMethod.KaltofenShoup;

            //var dir = @"valid-check-wolfram\";

            var fieldOrderPath = Path.Combine(AppContext.BaseDirectory, "fieldOrder_input.txt");
            var inputPolsPath = Path.Combine(AppContext.BaseDirectory, "pols_input.txt");
            var scriptFilePath = Path.Combine(AppContext.BaseDirectory, "factorPols.wls");

            var inputPols = GeneratePolsOverGF(gf, deg, numPols, false); //GenerateMSFPolsOverGF(gf, deg, numPols);

            // save field order to file
            using (var sw = new StreamWriter(fieldOrderPath))
            {
                sw.WriteLine(gf.ToString());
            }

            // save pols to file
            using (var sw = new StreamWriter(inputPolsPath))
            {
                for (int i = 0; i < inputPols.Count; i++)
                {
                    var pol = inputPols[i];

                    if (i == inputPols.Count - 1)
                        sw.Write(pol.ToList());
                    else
                        sw.WriteLine(pol.ToList());
                }
            }

            var engine = new WolframEngine();

            // create: pols_output.json using wolfram
            engine.ExecuteFile(scriptFilePath);

            // factor using this library


            // read result from pols_output.json:
            var pathToJson = Path.Combine(AppContext.BaseDirectory, "pols_output.json");
            var jsonString = File.ReadAllText(pathToJson);

            var jso = new JsonSerializerOptions();
            jso.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
            var polsJson = JsonSerializer.Deserialize<List<List<FactorPart>>>(jsonString, jso);

            int j = 0;

            var wfList = new List<(ulong, List<(GFPoly, int)>)>();
            var libList = new List<(ulong, List<(GFPoly, int)>)>();

            foreach (var fp in polsJson)
            {
                var wolframFactors = ConvertFactorFromJsonToGFPoly(fp, gf);
                var libFactors = inputPols[j++].Factor(method);

                wfList.Add(wolframFactors);
                libList.Add(libFactors);
            }

            return (wfList, libList);
        }

        private static bool CompareFactors(ulong c1, ulong c2, List<(GFPoly, int)> fs1, List<(GFPoly, int)> fs2)
        {
            if (c1 != c2)
                return false;

            if (fs1.Count != fs2.Count)
                return false;

            int n = fs1.Count;
            var visited = new bool[n];

            var comparePols = ((GFPoly, int) f1, (GFPoly, int) f2)
                => f1.Item1 == f2.Item1 && f1.Item2 == f2.Item2;

            for (int i = 0; i < n; i++)
            {
                // find fs1[i] in list -> 'fs2'
                for (int j = 0; j < n; j++)
                {
                    if (comparePols(fs1[i], fs2[j]) && !visited[j])
                    {
                        visited[j] = true;
                        break;
                    }

                    if (j == n - 1)
                        return false;
                }
            }

            return true;
        }

        [Fact]
        public void CheckValidBerlekamp()
        {
            var gf = new GF(3); // 1299709
            int startDeg = 10;
            int endDeg = 10;
            int numPols = (int)Math.Pow(gf.Order, startDeg); //(int)Math.Pow(gf.Order, deg + 1);

            var method = FactorMethod.Berlekamp;

            for (int j = startDeg; j <= endDeg; j++)
            {
                int deg = j;

                var factorLists = CheckValid(gf, deg, numPols, method);

                // compare results
                for (int i = 0; i < factorLists.Item1.Count; i++)
                {
                    var wfList = factorLists.Item1[i];
                    var libList = factorLists.Item2[i];

                    bool isEqual = CompareFactors(libList.Item1, wfList.Item1, libList.Item2, wfList.Item2);

                    Assert.True(isEqual);
                }
            }
        }

        [Fact]
        public void CheckValidCantorZassenhaus()
        {
            var gf = new GF(3); // 1299709
            int startDeg = 5;
            int endDeg = 16;
            int numPols = 1000;//(int)Math.Pow(gf.Order, deg + 1);

            var method = FactorMethod.CantorZassenhaus;

            for (int j = startDeg; j <= endDeg; j++)
            {
                int deg = j;

                var factorLists = CheckValid(gf, deg, numPols, method);

                for (int i = 0; i < factorLists.Item1.Count; i++)
                {
                    var wfList = factorLists.Item1[i];
                    var libList = factorLists.Item2[i];

                    bool isEqual = CompareFactors(libList.Item1, wfList.Item1, libList.Item2, wfList.Item2);

                    Assert.True(isEqual);
                }
            }
        }

        [Fact]
        public void CheckValidKaltofenShoup()
        {
            var gf = new GF(3); // 1299709
            int startDeg = 5;
            int endDeg = 16;
            int numPols = 1000;//(int)Math.Pow(gf.Order, deg + 1);

            var method = FactorMethod.KaltofenShoup;

            for (int j = startDeg; j <= endDeg; j++)
            {
                int deg = j;

                var factorLists = CheckValid(gf, deg, numPols, method);

                for (int i = 0; i < factorLists.Item1.Count; i++)
                {
                    var wfList = factorLists.Item1[i];
                    var libList = factorLists.Item2[i];

                    bool isEqual = CompareFactors(libList.Item1, wfList.Item1, libList.Item2, wfList.Item2);

                    Assert.True(isEqual);
                }
            }
        }
    }
}
