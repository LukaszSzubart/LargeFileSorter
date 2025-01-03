using Sorter.Common;

namespace Sorter.Phase1;
internal static class Phase1Orchestrator
{
    public static async Task<PersistentChunkInfo[]> ExecutePhase1(RunInfo runInfo)
    {
        var chunkReadInfos = await Phase1ChunkInfoFactory.Create(runInfo);
        await ReadSortAndWriteToDiskInParallel(chunkReadInfos);

        return chunkReadInfos.Select(c => c.Destination).ToArray();
    }


    private static async Task ReadSortAndWriteToDiskInParallel(IReadOnlyList<Phase1ChunkInfo> chunkInfos)
    {
        var options = new ParallelOptions
        {
            MaxDegreeOfParallelism = GlobalSettings.ThreadLimit
        };
        await Parallel.ForEachAsync(chunkInfos, options, ReadSortAndWriteToDisk);
    }


    private static async ValueTask ReadSortAndWriteToDisk(Phase1ChunkInfo chunkInfo, CancellationToken token)
    {
        using var data = await VirtualChunkReader.Read(chunkInfo.Source);
        Array.Sort(data.Rows, 0, chunkInfo.Source.RowCount, Row.Comparer);
        PersistentChunkStreamWriter.Write(chunkInfo.Destination, data.Rows);

        if (GlobalSettings.DumpIntermidieteChunkDefinition)
        {
            await IntermediateChunkInfoDumper.Dump("1", chunkInfo.Destination);
        }
    }
}