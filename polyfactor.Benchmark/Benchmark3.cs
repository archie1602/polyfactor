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
    [SimpleJob(RuntimeMoniker.Net60)]
    [Config(typeof(Config))]
    public class Benchmark3
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

                //SummaryStyle = new SummaryStyle(new CultureInfo("en-US"),
                //                                    false,
                //                                    SizeUnit.B,
                //                                    Perfolizer.Horology.TimeUnit.Second);

                //SummaryStyle = SummaryStyle.Default.WithRatioStyle(RatioStyle.Trend);
            }
        }

        private readonly GFPoly[] _pols = null;
        private readonly GF _gf = new GF(3);
        private readonly int _deg = 10;

        public Benchmark3()
        {
            _pols = GeneratePols(_gf, _deg);
            Console.WriteLine("Length: " + _pols.Length);
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

                pols[i] = pol;

                t = GetNextPol(t.coeffs, p);
                i++;
            }

            return pols;
        }

        //[Benchmark(Baseline = true)]
        [Benchmark]
        public void Factor_Berlekamp()
        {
            for (int i = 0; i < _pols.Length; i++)
                _pols[i].Factor(FactorMethod.Berlekamp, false);
        }

        //[Benchmark]
        //public void Factor_CantorZassenhaus()
        //{
        //    for (int i = 0; i < _pols.Length; i++)
        //        _pols[i].Factor(FactorMethod.CantorZassenhaus, false);
        //}
    }
}
