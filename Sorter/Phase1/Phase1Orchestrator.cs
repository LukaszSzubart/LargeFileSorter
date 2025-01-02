using Sorter.Common;

namespace Sorter.Phase1;
internal static class Phase1Orchestrator
{
    public static async Task<IEnumerable<IntermediateChunkInfo>> ExecutePhase1(RunInfo runInfo)
    {
        var chunkReadInfos = await Phase1ChunkInfoFactory.Create(runInfo);
        await ReadSortAndWriteToDiskInParallel(chunkReadInfos);

        return chunkReadInfos.Select(c => c.DestinationChunkInfo);
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
        using var chunkData = await SourceChunkReader.Read(chunkInfo.Source);
        Array.Sort(chunkData.LineBuffer, 0, chunkInfo.Source.LineCount, Row.Comparer);
        await IntermidiateChunkStreamWriter.Write(chunkInfo.DestinationChunkInfo, chunkData.LineBuffer);

        if (GlobalSettings.DumpIntermidieteChunkDefinition)
        {
            await IntermediateChunkInfoDumper.Dump("1", chunkInfo.DestinationChunkInfo);
        }
    }
}