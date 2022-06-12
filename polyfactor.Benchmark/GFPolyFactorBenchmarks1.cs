using System;
using System.IO;
using System.Linq;
using System.Text.Json;
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
    [SimpleJob(RuntimeMoniker.Net60, warmupCount: 1, targetCount: 2, launchCount: 1)]
    [Config(typeof(Config))]
    public class GFPolyFactorBenchmarks1
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

                WithSummaryStyle(style);
            }
        }

        private readonly string _pathToDir = @"C:\Users\archie\Desktop\MainDir\Desktop\CodeProjs\University\BachelorThesis\Sem8\polyfactor\polyfactor.ConsoleAppTesting\data";

        private List<GFPoly> _sfMonicPols = null;

        private const int _startDeg = 1000;
        private const int _stepDeg = 10;
        private const int _endDeg = 1000;

        public IEnumerable<FactorMethod> FactorMethodValues => new FactorMethod[]
        {
            //FactorMethod.CantorZassenhaus,
            FactorMethod.KaltofenShoup,
            //FactorMethod.Berlekamp
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
                //new GF(101)
            //new GF(131),
            //new GF(1009)
            //new GF(359),
            //new GF(607),
            //new GF(809),
            //new GF(1009)
            //new GF(1021)
            //new GF(2069),
            //new GF(3697),
            //new GF(4547),
            //new GF(5237),
            //new GF(6829),
            //new GF(7919),
            //new GF(10_007)
            //new GF(20029)
            new GF(65_537)
            //new GF(10_069)
        };

        public IEnumerable<int> PolDegValues()
        {
            for (int deg = _startDeg; deg <= _endDeg; deg += _stepDeg)
            {
                yield return deg;
            }
        }

        [ParamsSource(nameof(FactorMethodValues))]
        public FactorMethod Method { get; set; }

        [ParamsSource(nameof(GaloisFieldValues))]
        public GF Field { get; set; }

        [ParamsSource(nameof(PolDegValues))]
        public int Deg { get; set; }

        private static GFPoly CreatePolFromListStrRepr(GF gf, string polListStr)
        {
            var polStrTmp = polListStr.Substring(1, polListStr.Length - 2).Replace(',', ' ');
            var polStrCoeffs = polStrTmp.Split(' ');

            var coeffs = polStrCoeffs.Select(c => ulong.Parse(c)).ToArray();

            return new GFPoly(gf, coeffs);
        }

        [GlobalSetup]
        public void GlobalSetup()
        {
            // read from file
            using (var fs = new FileStream(Path.Combine(_pathToDir, $"input-pols-GF-{Field}.json"), FileMode.OpenOrCreate))
            {
                var jsonRead = JsonSerializer.Deserialize<Dictionary<int, List<string>>>(fs);

                var strPols = jsonRead[Deg];
                _sfMonicPols = new List<GFPoly>();

                foreach (var pol in strPols)
                {
                    _sfMonicPols.Add(CreatePolFromListStrRepr(Field, pol));
                }

                Console.WriteLine($"Read => Deg: {Deg}; Field: {Field.Order}");
            }
        }

        [Benchmark]
        public void Factor()
        {
            for (int i = 0; i < _sfMonicPols.Count; i++)
            {
                var factors = _sfMonicPols[i].FactorMSFPol(Method);
            }
        }
    }
}