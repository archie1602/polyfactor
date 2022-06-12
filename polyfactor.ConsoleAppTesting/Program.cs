using polyfactor.Test.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Text.Json;
using System.Text.Encodings.Web;
using System.IO;
using System.Threading.Tasks;
//using Wolfram.NETLink;

using polyfactor.GaloisStructs;

namespace polyfactor.ConsoleAppTesting
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            return;

            var fields = new GF[]
            {
                //new GF(7, false),
                //new GF(23, false),
                //new GF(59, false),
                //new GF(101, false),
                //new GF(131, false)
                //new GF(1009, false)
                new GF(65_537, false)
                //new GF(10_069, false)
            };

            int startDeg = 1000;
            int stepDeg = 10;
            int endDeg = 1000;
            int num = 1;

            foreach (var gf in fields)
            {
                var pathToFile = @"C:\Users\archie\Desktop\MainDir\Desktop\CodeProjs\University\BachelorThesis\Sem8\polyfactor\polyfactor.ConsoleAppTesting\data";

                pathToFile = Path.Combine(pathToFile, $"input-pols-GF-{gf}.json");

                var strcts = new Dictionary<int, List<string>>();

                //var isExists = File.Exists(pathToFile);

                for (int deg = startDeg; deg <= endDeg; deg += stepDeg)
                {
                    var rndPols = GenerateRandomSFPols(gf, deg, num);

                    strcts.Add(deg, rndPols);
                }

                // save data to json
                using (var fs = new FileStream(pathToFile, FileMode.OpenOrCreate))
                {
                    await JsonSerializer.SerializeAsync(fs, strcts);
                    Console.WriteLine($"Data has been saved to file with field: {gf.Order}");
                }
            }

            //// read from file
            //using (var fs = new FileStream(pathToFile, FileMode.OpenOrCreate))
            //{
            //    var jsonRead = await JsonSerializer.DeserializeAsync<Dictionary<int, List<string>>>(fs);

            //    var polsDict = new Dictionary<int, List<GFPoly>>();

            //    foreach(var r in jsonRead)
            //    {
            //        var currPols = new List<GFPoly>();

            //        foreach (var p in r.Value)
            //            currPols.Add(CreatePolFromListStrRepr(gf, p));

            //        polsDict.Add(r.Key, currPols);
            //    }

            //    Console.WriteLine("Read");
            //}

            //var jsonRead = JsonSerializer.Deserialize<List<PolListJsonSave>>(jsonWrite);

        }

        private static List<string> GenerateRandomSFPols(GF gf, int deg, int num)
        {
            var pols = new List<string>(num);
            int count = 0;

            while(count < num)
            {
                var randPol = GFPoly.GenerateRandomMonicPol(gf, deg);

                if (randPol.IsSquareFree)
                {
                    pols.Add(randPol.ToString());
                    count++;
                }
            }

            return pols;
        }

        private static GFPoly CreatePolFromListStrRepr(GF gf, string polListStr)
        {
            //var polStr = "{61681,36635,9178,22854,37941,42925,20344,8683,63428,1}";
            var polStrTmp = polListStr.Substring(1, polListStr.Length - 2).Replace(',', ' ');
            var polStrCoeffs = polStrTmp.Split(' ');

            var coeffs = polStrCoeffs.Select(c => ulong.Parse(c)).ToArray();

            return new GFPoly(gf, coeffs);
        }

        private static void Main6(string[] args)
        {
            var gf = new GF(3);
            var deg = 10;

            var arr = GeneratePols(gf, deg);

            var list = new List<(ulong coeff, List<(GFPoly, int)> irrFactors)>();

            for (int i = 0; i < arr.Length; i++)
                list.Add(arr[i].Factor(FactorMethod.Berlekamp, false));

            int a = 5;
        }

        private static void Main5(string[] args)
        {
            var gf = new GF(65537);

            var coeffs = new ulong[] { 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 };

            var pol = new GFPoly(gf, coeffs);

            pol.Factor(FactorMethod.CantorZassenhaus, false);
        }

        private static (bool flag, ulong[] coeffs) GetNextPol(ulong[] arr, ulong p)
        {
            var coeffs = new ulong[arr.Length];

            // copy 'arr' to 'coeffs'
            for (int i = 0; i < arr.Length; i++)
                coeffs[i] = arr[i];

            for (int i = 0; i < arr.Length; i++)
            {
                if (coeffs[i] == p - 1)
                {
                    coeffs[i] = 0;
                    continue;
                }

                coeffs[i]++;
                return (true, coeffs);
            }

            return (false, coeffs);
        }

        private static GFPoly[] GeneratePols(GF gf, int deg)
        {
            var init = new ulong[deg + 1];

            init[^1] = 1;

            ulong p = gf.Order;

            (bool flag, ulong[] coeffs) t = (true, init);

            int numPols = (int)Math.Pow(gf.Order, deg);

            var pols = new GFPoly[numPols];

            int i = 0;

            while (t.flag && i < numPols)
            {
                var pol = new GFPoly(gf, t.coeffs);
                //Console.WriteLine($"{i + 1}. " + pol.ToList());

                pols[i] = pol;

                t = GetNextPol(t.coeffs, p);
                i++;
            }

            return pols;
        }

        private static void Main4(string[] args)
        {
            var gf = new GF(3); // 19

            var coeffs = new ulong[]
            { 1, 1, 1, 7, 1, 7, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 4, 1 };

            var polStr = "x^4+x^3+x+2";

            var pol = new GFPoly(gf, polStr);

            pol.FactorMSFPol(FactorMethod.CantorZassenhaus);
        }

        private static void Main3(string[] args)
        {
            //var polTest = new GFPoly(new(5), "0");

            //var ressss = polTest.Factor();

            //return;
            string pathToJson = @"C:\Users\archie\Desktop\MainDir\Desktop\CodeProjs\University\BachelorThesis\Sem8\forTesting\wolfram\pols_output.json";

            var res = new List<List<FactorPart>>()
            {
                new List<FactorPart>()
                {
                    new() { pol = "x^2+1", exp = "2" },
                    new() { pol = "x^3+1", exp = "1" },
                    new() { pol = "x^5+1", exp = "5" }
                },
                new List<FactorPart>()
                {
                    new() { pol = "x^2+1", exp = "2" },
                    new() { pol = "x^3+1", exp = "1" },
                    new() { pol = "x^5+1", exp = "5" }
                }
            };

            var jso = new JsonSerializerOptions();
            jso.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
            var ser = JsonSerializer.Serialize(res, jso);


            var jsonString = File.ReadAllText(pathToJson);

            var pols = JsonSerializer.Deserialize<List<List<FactorPart>>>(jsonString, jso);

            foreach (var f in pols)
            {
                var resConvert = ConvertToInternalReprs(f, new(3));
            }
        }

        public static (ulong coeff, List<(GFPoly, int)> irrFactors) ConvertToInternalReprs(List<FactorPart> factors, GF gf)
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

        private static void Main2(string[] args)
        {
            var gf = new GF(29);
            int deg = 10;
            int num = 1000;

            var sfMonicPols = GenerateMSFPolsOverGF(gf, deg, num);

            int i = 1;

            foreach (var p in sfMonicPols)
                Console.WriteLine($"{i++}. \t" + p);
        }

        public static (bool flag, ulong[] coeffs) GetNextPol(ulong[] arr, ulong p, int lastDeg)
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

        /// <summary>
        /// generates numPols count monic square-free pols over gf with specific deg
        /// </summary>
        /// <param name="gf"></param>
        /// <param name="deg"></param>
        /// <param name="numPols"></param>
        /// <returns></returns>
        public static List<GFPoly> GenerateMSFPolsOverGF(GF gf, int deg, int numPols)
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
    }
}
