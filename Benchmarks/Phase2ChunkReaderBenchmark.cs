using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using Benchmarks.TestData;
using Sorter;
using Sorter.Common;
using Sorter.Phase2;

namespace Benchmarks;

//| Method  | Mean     | Error    | StdDev   | Gen0      | Gen1      | Gen2     | Allocated |
//|-------- |---------:|---------:|---------:|----------:|----------:|---------:|----------:|
//| Simple  | 38.23 ms | 0.730 ms | 1.603 ms | 1785.7143 | 1285.7143 | 642.8571 |  15.85 MB |
//| Channel | 37.47 ms | 0.727 ms | 1.132 ms | 1928.5714 | 1571.4286 | 857.1429 |  15.88 MB |

[MemoryDiagnoser]
[SimpleJob(RuntimeMoniker.Net90)]
public class Phase2ChunkReaderBenchmark
{
    private IntermediateChunkInfo _chunkInfo = TestDataInfos.ChunkSmallSorted;


    [Benchmark]
    public async Task<object> Simple()
    {
        using var reader = new Phase2SimpleChunkReader(_chunkInfo);
        return await PerformTest(reader);
    }

    [Benchmark]
    public async Task<object> Channel()
    {
        var reader = new Phase2ChannelChunkReader(_chunkInfo);
        return await PerformTest(reader);
    }

    private static async Task<object> PerformTest(IPhaseChunkReader reader)
    {
        var rows = new List<Row>(GlobalSettings.ArrayPoolLengthLimit);

        while (!reader.Completed)
        {
            await reader.Reload();
            rows.Add(reader.Row);
        }
        return rows;
    }
}
