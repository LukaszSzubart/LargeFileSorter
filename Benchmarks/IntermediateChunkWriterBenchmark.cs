using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using Benchmarks.TestData;
using Sorter.Common;
using System.Text.Json;

namespace Benchmarks;


//| Method       | Mean       | Error     | StdDev     | Median     | Gen0     | Gen1     | Gen2     | Allocated  |
//|------------- |-----------:|----------:|-----------:|-----------:|---------:|---------:|---------:|-----------:|
//| Pipe         | 245.145 ms | 4.8884 ms | 10.5228 ms | 246.366 ms | 500.0000 |        - |        - | 2583.79 KB |
//| StreamWriter | 213.658 ms | 4.2687 ms |  7.4762 ms | 208.620 ms | 333.3333 | 333.3333 | 333.3333 | 2358.92 KB |
//| Stream       |   7.726 ms | 0.1517 ms |  0.2176 ms |   7.663 ms |        - |        - |        - |    4.49 KB |


[MemoryDiagnoser]
[SimpleJob(RuntimeMoniker.Net90)]
public class IntermediateChunkWriterBenchmark
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    private Row[] _rows;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        
    [GlobalSetup]
    public void Setup()
    {
        using var sourceStream = File.OpenRead(TestDataPaths.RowsSmallJson);
        _rows = JsonSerializer.Deserialize<Row[]>(sourceStream);
    }

    [Benchmark]
    public async Task Pipe()
    {
        await IntermidiateChunkPipeWriter.Write(GetInfo(nameof(Pipe)), _rows);
    }

    [Benchmark]
    public async Task StreamWriter()
    {
        await IntermidiateChunkStreamWriterWriter.Write(GetInfo(nameof(StreamWriter)), _rows);
    }

    [Benchmark]
    public async Task Stream()
    {
        await IntermidiateChunkStreamWriter.Write(GetInfo(nameof(Stream)), _rows);
    }

    private IntermediateChunkInfo GetInfo(string name)
    {
        var path = Path.Combine(TestDataPaths.OutDir, '_' + name + ".txt");
        return new IntermediateChunkInfo(1, path, _rows.Length, 1);
    }
}
