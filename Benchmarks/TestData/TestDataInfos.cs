using Sorter.Common;
using Tools;

namespace Benchmarks.TestData;
internal static class TestDataInfos
{
    public static PersistentChunkInfo ChunkSmallSorted { get; } = new(
        -1,
        TestDataPaths.ChunkSmallSorted,
        69266,
        2850 * BinarySize.InBytes.KB);
}
