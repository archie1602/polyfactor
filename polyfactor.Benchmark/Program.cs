using BenchmarkDotNet.Running;

namespace polyfactor.Benchmark
{
    public class Program
    {
        private static void Main(string[] args)
        {
            BenchmarkRunner.Run<GFPolyFactorBenchmarks1>();
            //BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
        }
    }
}