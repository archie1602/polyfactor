using System;
using System.Linq;
using System.Threading;
using System.Globalization;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.Concurrent;

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Exporters.Csv;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Mathematics;
using BenchmarkDotNet.Reports;

using polyfactor.GaloisStructs;

namespace polyfactor.Benchmark
{
    [SimpleJob( RuntimeMoniker.Net60, warmupCount: 1, targetCount: 1, launchCount: 1)]
    [Config(typeof(Config))]
    public class GFPolyFactorBenchmarks
    {
        private class Config : ManualConfig
        {
            public Config()
            {
                AddExporter(CsvMeasurementsExporter.Default);
                AddExporter(RPlotExporter.Default);
                AddExporter(HtmlExporter.Default);
                AddDiagnoser(new MemoryDiagnoser(new MemoryDiagnoserConfig(true)));
                AddColumn(new RankColumn(NumeralSystem.Arabic));

                var style = new SummaryStyle(new CultureInfo("en-US"),
                                                    false,
                                                    SizeUnit.B,
                                                    Perfolizer.Horology.TimeUnit.Millisecond);

                //AddJob(Job.MediumRun.WithGcServer(true).WithGcForce(true).WithId("ServerForce"));
                //AddJob(Job.MediumRun.WithGcServer(true).WithGcForce(false).WithId("Server"));
                //AddJob(Job.MediumRun.WithGcServer(false).WithGcForce(true).WithId("Workstation"));
                //AddJob(Job.MediumRun.WithGcServer(false).WithGcForce(false).WithId("WorkstationForce"));

                WithSummaryStyle(style);
            }
        }

        //private readonly Dictionary<int, List<GFPoly>> _sfMonicPols = new Dictionary<int, List<GFPoly>>();
        private GFPoly _nxnPol = null;

        //private const int _startDeg = 4;
        //private const int _endDeg = 17;

        private const int _num = 1000;

        public IEnumerable<FactorMethod> FactorMethodValues => new FactorMethod[]
        {
            FactorMethod.KaltofenShoup,
            FactorMethod.CantorZassenhaus,
            FactorMethod.Berlekamp
        };

        public IEnumerable<GF> GaloisFieldValues => new GF[]
        {
            //new GF(3),
            //new GF(5),
            //new GF(7),
            //new GF(11)
            //new GF(13),
            //new GF(17),
            //new GF(23),
            //new GF(29),
            //new GF(17),
            //new GF(11),
            //new GF(257),
            //new GF(47),
            //new GF(59),
            //new GF(101),
            //new GF(131)
            //new GF(359),
            //new GF(607),
            //new GF(809),
            new GF(1009)
            //new GF(1021)
            //new GF(2069),
            //new GF(3697),
            //new GF(4547),
            //new GF(5237),
            //new GF(6829),
            //new GF(7919),
            //new GF(10_007)
            //new GF(20029)
            //new GF(65537)
        };

        //public IEnumerable<int> PolDegValues()
        //{
        //    //for (int deg = _startDeg; deg <= _endDeg; deg++)
        //    //    yield return deg;

        //    int count = 2;
        //    int[] initDegs = new int[] { 5, 10 };

        //    int len = initDegs.Length * count;

        //    for (int i = 0; i < len; i++)
        //    {
        //        yield return initDegs[i % initDegs.Length] * (int)Math.Pow(10, i / initDegs.Length);
        //    }
        //}

        public IEnumerable<int> PolDegValues()
        {
            for (int i = 100; i <= 1000; i += 100)
            {
                yield return i;
            }
        }

        //public IEnumerable<int> PolDegValues => new int[]
        //{
        //    //10, 30, 50, 70, 90, 110
        //    //10, 20, 30, 40, 50, 60, 70, 80, 90, 100
        //    //5, 20, 35, 50, 65, 80, 95, 110
        //    3, 6, 9, 12, 15, 18, 21, 24, 27, 30, 33, 36, 39, 42
        //};

        //public IEnumerable<int> PolDegValues => new int[]
        //{
        //    // 5, 10, 50, 100, 500, 1000, 5000, 10000
        //};

        [ParamsSource(nameof(FactorMethodValues))]
        public FactorMethod Method { get; set; }

        [ParamsSource(nameof(GaloisFieldValues))]
        public GF Field { get; set; }

        [ParamsSource(nameof(PolDegValues))]
        public int Deg { get; set; }

        [GlobalSetup]
        public void GlobalSetup()
        {
            //for (int i = _startDeg; i <= _endDeg; i++)
            //    _sfMonicPols[i] = GenerateMSFPolsOverGF(Field, i, _num);

            //int count = 2;
            //int[] initDegs = new int[] { 5, 10 };

            //int len = initDegs.Length * count;

            //for (int i = 0; i < len; i++)
            //{
            //    int deg = initDegs[i % initDegs.Length] * (int)Math.Pow(10, i / initDegs.Length);

            //    _sfMonicPols[deg] = GenerateMSFPolsOverGF(Field, deg, _num);
            //}

            //var degs = new int[]
            //{
            //    //10, 30, 50, 70, 90, 110
            //    //10, 20, 30, 40, 50, 60, 70, 80, 90, 100
            //    //5, 20, 35, 50, 65, 80, 95, 110
            //    3, 6, 9, 12, 15, 18, 21, 24, 27, 30, 33, 36, 39, 42
            //};

            //for (int i = 0; i < degs.Length; i++)
            //    _sfMonicPols[degs[i]] = GenerateMSFPolsOverGF(Field, degs[i], _num);

            //for (int i = 5; i <= 100; i += 5)
            //    _sfMonicPols[i] = GenerateMSFPolsOverGF(Field, i, _num);

            Console.WriteLine($"Deg: {Deg}; Field: {Field}");
            _nxnPol = GeneratePol_nXn(Deg, Field);
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

        /// <summary>
        /// generates numPols count monic square-free pols over gf with specific deg
        /// </summary>
        /// <param name="gf"></param>
        /// <param name="deg"></param>
        /// <param name="numPols"></param>
        /// <returns></returns>
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

        private static GFPoly GeneratePol_nXn(int n, GF field)
        {
            var coeffs = new ulong[n + 1];

            coeffs[0] = 0;
            coeffs[n] = (ulong)n;

            for (uint i = 1; i < n; i++)
                coeffs[i] = i;

            return new GFPoly(field, coeffs);
        }

        //[Benchmark]
        //public void Factor()
        //{
        //    var pols = _sfMonicPols[Deg];

        //    for (int i = 0; i < pols.Count; i++)
        //        pols[i].FactorMSFPol(Method);
        //}

        [Benchmark]
        public void Factor()
        {
            _nxnPol.Factor(Method, false);
        }
    }
}