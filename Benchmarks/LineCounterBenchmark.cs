using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using Benchmarks.TestData;
namespace Benchmarks;


//| Method               | Mean     | Error     | StdDev    | Ratio | Gen0     | Gen1     | Gen2     | Allocated  | Alloc Ratio |
//|--------------------- |---------:|----------:|----------:|------:|---------:|---------:|---------:|-----------:|------------:|
//| Stream               | 2.695 ms | 0.0178 ms | 0.0158 ms |  1.00 | 332.0313 | 332.0313 | 332.0313 | 1024.44 KB |       1.000 |
//| PipeWithTryAdvanceTo | 1.292 ms | 0.0040 ms | 0.0034 ms |  0.48 |        - |        - |        - |    1.62 KB |       0.002 |
//| PipeWithReadTo       | 1.307 ms | 0.0055 ms | 0.0052 ms |  0.49 |        - |        - |        - |    1.75 KB |       0.002 |


[MemoryDiagnoser]
[SimpleJob(RuntimeMoniker.Net90)]
public class LineCounterBenchmark
{
    [Benchmark(Baseline = true)]
    public long Stream()
    {
        using var stream = File.OpenRead(TestDataPaths.ChunkSmallSorted);
        return Tools.StreamLineCounter.Count(stream);
    }

    [Benchmark]
    public async Task<long> PipeWithTryAdvanceTo()
    {
        using var stream = File.OpenRead(TestDataPaths.ChunkSmallSorted);
        return await Tools.PipeLineCounter.CountWithTryAdvanceTo(stream);
    }

    [Benchmark]
    public async Task<long> PipeWithReadTo()
    {
        using var stream = File.OpenRead(TestDataPaths.ChunkSmallSorted);
        return await Tools.PipeLineCounter.CountWithTryReadto(stream);
    }
}
