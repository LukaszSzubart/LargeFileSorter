using Sorter.Common;

namespace Sorter.Phase2;

internal record LazyChunkData(PersistentChunkInfo Info, IAsyncEnumerator<Row> Reader)
{
    public static LazyChunkData[] CreateFrom(PersistentChunkInfo[] sourceInfos)
    {
        var result = new LazyChunkData[sourceInfos.Length];
        for (int i = 0; i < result.Length; i++)
        {
            var info = sourceInfos[i];
            result[i] = new LazyChunkData(info, new PersistentChunkBufferedLazyReader(info));
        }
        return result;
    }
}
