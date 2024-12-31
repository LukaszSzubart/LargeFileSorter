using Sorter.Phase0;

namespace Sorter.Phase1;

internal class ChankSorter
{
    public async Task InitialSort(ChunkInfo[] chunks)
    {
        var options = new ParallelOptions
        {
            MaxDegreeOfParallelism = Settings.ThreadLimit
        };
        await Parallel.ForEachAsync(chunks, options, SortChunk);
    }


    private async ValueTask SortChunk(ChunkInfo chunk, CancellationToken token)
    {

    }
}
