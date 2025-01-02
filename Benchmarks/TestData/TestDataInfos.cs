using Sorter.Common;
using Tools;

namespace Benchmarks.TestData;
internal static class TestDataInfos
{
    public static IntermediateChunkInfo ChunkSmallSorted { get; } = new(
        -1,
        TestDataPaths.ChunkSmallSorted,
        69267,
        2850 * BinarySize.InBytes.KB);
}
