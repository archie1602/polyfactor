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
    public class Benchmark2
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
                                                    Perfolizer.Horology.TimeUnit.Microsecond);

                //AddJob(Job.MediumRun.WithGcServer(true).WithGcForce(true).WithId("ServerForce"));
                //AddJob(Job.MediumRun.WithGcServer(true).WithGcForce(false).WithId("Server"));
                //AddJob(Job.MediumRun.WithGcServer(false).WithGcForce(true).WithId("Workstation"));
                //AddJob(Job.MediumRun.WithGcServer(false).WithGcForce(false).WithId("WorkstationForce"));

                WithSummaryStyle(style);
            }
        }

        private readonly GF _gf = new(17);
        private readonly Dictionary<int, GFPoly> _pols = new Dictionary<int, GFPoly>();

        [ParamsSource(nameof(_valuesForN))]
        public int N { get; set; }

        public IEnumerable<int> _valuesForN => new[]
        {
            10, //30,
            //100, 300,
            //1000, 3000,
            //10000, 30000
        };

        public Benchmark2()
        {
            foreach (var deg in _valuesForN)
                _pols.Add(deg, GeneratePol(_gf, deg));
        }

        private GFPoly GeneratePol(GF field, int n)
        {
            var coeffs = new ulong[n + 1];

            coeffs[0] = 0;

            for (uint i = 1; i <= n; i++)
                coeffs[i] = i;

            return new GFPoly(field, coeffs);
        }

        [Benchmark]
        public void Factor_CantorZassenhaus()
        {
            _pols[N].Factor(FactorMethod.CantorZassenhaus);
        }

        //[Benchmark]
        //public void Factor_KaltofenShoup()
        //{
        //    _pols[N].Factor(FactorMethod.KaltofenShoup);
        //}
    }
}
