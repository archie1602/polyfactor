using System;

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Jobs;

using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Exporters.Csv;

using polyfactor.GaloisStructs;

namespace polyfactor.Benchmark
{
    [HtmlExporter]
    [MemoryDiagnoser]
    [RankColumn]
    [SimpleJob(RuntimeMoniker.Net60)]
    [Config(typeof(Config))]
    public class Benchmark1
    {
        private class Config : ManualConfig
        {
            public Config()
            {
                AddExporter(CsvMeasurementsExporter.Default);
                AddExporter(RPlotExporter.Default);
            }
        }

        private readonly GFPoly _pol;
        private readonly GF _gf = new(17);

        public Benchmark1()
        {
            _pol = GeneratePol(1000, _gf);

            Console.WriteLine("=========> " + _pol.IsSquareFree);
        }

        private GFPoly GeneratePol(int n, GF field)
        {
            var coeffs = new ulong[n + 1];

            coeffs[0] = 0;
            coeffs[n] = 1;

            for (uint i = 1; i < n; i++)
                coeffs[i] = i;

            return new(field, coeffs);
        }

        //[Benchmark]
        //public void FactorDeg100_CantorZassenhaus()
        //{
        //    _pol.FactorMSFPol(FactorMethod.CantorZassenhaus);
        //}

        [Benchmark]
        public void FactorDeg100_KaltofenShoup()
        {
            _pol.FactorMSFPol(FactorMethod.KaltofenShoup);
        }
    }
}
