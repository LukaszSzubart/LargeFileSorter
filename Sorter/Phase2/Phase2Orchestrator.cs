using Sorter.Common;

namespace Sorter.Phase2;
internal static class Phase2Orchestrator
{
    
    public static async Task<PersistentChunkInfo> ExecutePhase2(PersistentChunkInfo[] sourceInfos)
    {
        var destChunkInfo = PrepareDestChunkInfo(sourceInfos);
        var sourceChunks = LazyChunkData.CreateFrom(sourceInfos);

        var minHeap = await PrepareMinHeap(sourceChunks);

        using var writer = PersistentChunkStreamWriter.Create(destChunkInfo);

        while(minHeap.TryDequeue(out var chunk, out _))
        {
            writer.Write(chunk.Reader.Current);
            if(await chunk.Reader.MoveNextAsync())
            {
                minHeap.Enqueue(chunk, chunk.Reader.Current);
            }
        }

        foreach (var chunk in sourceChunks)
        {
            await chunk.Reader.DisposeAsync();
        }

        return destChunkInfo;
    }

    private static async Task<PriorityQueue<LazyChunkData, Row>> PrepareMinHeap(LazyChunkData[] chunks)
    {
        var priorityQueue = new PriorityQueue<LazyChunkData, Row>(chunks.Length, Row.Comparer);

        foreach (var chunk in chunks)
        {
            await chunk.Reader.MoveNextAsync();
            priorityQueue.Enqueue(chunk, chunk.Reader.Current);
        }

        return priorityQueue;
    }

    private static PersistentChunkInfo PrepareDestChunkInfo(PersistentChunkInfo[] sourceInfos)
    {
        var totalRowCount = 0L;
        var totalSize = 0L;

        for (var i = 0; i < sourceInfos.Length; i++)
        {
            var info = sourceInfos[i];
            totalRowCount += info.RowCount;
            totalSize += info.SizeInBytes;
        }

        var path = TempFilePathFactory.CreateChunkFilePath("2", int.MaxValue);

        return new PersistentChunkInfo(int.MaxValue, path, totalRowCount, totalSize);
    }
}
