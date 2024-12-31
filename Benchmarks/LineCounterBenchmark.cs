using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
namespace Benchmarks;

[MemoryDiagnoser]
[SimpleJob(RuntimeMoniker.Net90)]
public class LineCounterBenchmark
{
    private const string Dir = "C:\\code\\LargeFileSorter\\TestFileGenerator\\bin\\Release\\net9.0\\";
    private const string Path = Dir + "1024mb.txt";

    [Benchmark(Baseline = true)]
    public long StreamLineCounter()
    {
        using var stream = File.OpenRead(Path);
        return Tools.StreamLineCounter.Count(stream);
    }

    [Benchmark]
    public async Task<long> PipeLineCounter()
    {
        using var stream = File.OpenRead(Path);
        return await Tools.PipeLineCounter.Count(stream);
    }

    [Benchmark]
    public async Task<long> PipeLineCounterWithReadTo()
    {
        using var stream = File.OpenRead(Path);
        return await Tools.PipeLineCounter.CountWithTryReadto(stream);
    }
}
