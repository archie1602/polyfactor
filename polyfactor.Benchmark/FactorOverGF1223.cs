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
    [HtmlExporter]
    [MemoryDiagnoser]
    [RankColumn]
    [SimpleJob(RuntimeMoniker.Net60)]
    [Config(typeof(Config))]
    public class FactorOverGF1223
    {
        private class Config : ManualConfig
        {
            public Config()
            {
                AddExporter(CsvMeasurementsExporter.Default);
                AddExporter(RPlotExporter.Default);
            }
        }

        private const int _startDeg = 3;
        private const int _endDeg = 14;
        private const int _num = 10;

        private const FactorMethod _bFM = FactorMethod.Berlekamp;
        private const FactorMethod _czFM = FactorMethod.CantorZassenhaus;
        private const FactorMethod _ksFM = FactorMethod.KaltofenShoup;

        public GF _gf => new(1223);

        private readonly Dictionary<int, List<GFPoly>> _sfMonicPols = new();

        public FactorOverGF1223()
        {
            _sfMonicPols[3] = GenerateMSFPolsOverGF(_gf, 3, _num);
            _sfMonicPols[4] = GenerateMSFPolsOverGF(_gf, 4, _num);
            _sfMonicPols[100] = GenerateMSFPolsOverGF(_gf, 100, _num);

            //_sfMonicPols = GenerateMonicPolsOverGF(_gf, _deg)
            //    .Where(p => p.IsSquareFree)
            //    .ToList();

            //Console.WriteLine($"Count square-free pols with deg {_deg}: {_sfMonicPols.Count}");
        }

        private (bool flag, ulong[] coeffs) GetNextPol(ulong[] arr, ulong p, int lastDeg)
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
        private List<GFPoly> GenerateMSFPolsOverGF(GF gf, int deg, int numPols)
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

        private void FactorDegMethod(int deg, FactorMethod method)
        {
            var pols = _sfMonicPols[deg];

            for (int i = 0; i < pols.Count; i++)
                pols[i].FactorMSFPol(method);
        }

        //#region Deg3

        //[Benchmark]
        //public void FactorDeg3_Berlekamp() => FactorDegMethod(3, _bFM);

        //[Benchmark]
        //public void FactorDeg3_CantorZassenhaus() => FactorDegMethod(3, _czFM);

        //[Benchmark]
        //public void FactorDeg3_KaltofenShoup() => FactorDegMethod(3, _ksFM);

        //#endregion

        //#region Deg4

        //[Benchmark]
        //public void FactorDeg4_Berlekamp() => FactorDegMethod(4, _bFM);

        //[Benchmark]
        //public void FactorDeg4_CantorZassenhaus() => FactorDegMethod(4, _czFM);

        //[Benchmark]
        //public void FactorDeg4_KaltofenShoup() => FactorDegMethod(4, _ksFM);

        //#endregion

        #region Deg100

        //[Benchmark]
        //public void FactorDeg100_Berlekamp() => FactorDegMethod(100, _bFM);

        [Benchmark]
        public void FactorDeg100_CantorZassenhaus() => FactorDegMethod(100, _czFM);

        [Benchmark]
        public void FactorDeg100_KaltofenShoup() => FactorDegMethod(100, _ksFM);

        #endregion
    }
}
