using System.Buffers;
using System.IO.Pipelines;

namespace Sorter.Phase0;

internal static class ChunkFactory
{
    private const char LF = '\n';

    public static async Task<ChunkInfo[]> Create(RunInfo runInfo)
    {
        ArgumentNullException.ThrowIfNull(runInfo);
        await using var inputFileStream = File.OpenRead(runInfo.InputFilePath);
        var pipeReader = PipeReader.Create(inputFileStream);
        
        List<ChunkInfo> chunks = new List<ChunkInfo>(512);
        
        ReadResult readResult;
        var totalConsumedBytes = 0L;
        var currentChunkStartPos = 0L;
        var currentChunkRowCount = 0;
        var totalRows = 0L;
        do
        {
            readResult = await pipeReader.ReadAsync();

            var sequenceReader = new SequenceReader<byte>(readResult.Buffer);

            while (sequenceReader.TryReadTo(out ReadOnlySpan<byte> bytes,(byte)LF, true))
            {
                totalConsumedBytes += bytes.Length;
                currentChunkRowCount++;
                totalRows++;
                var currentChunkSizeInBytes = totalConsumedBytes - currentChunkStartPos;
                if (ChunkLimitAchived(currentChunkSizeInBytes, currentChunkRowCount))
                {
                    chunks.Add(new (runInfo, currentChunkStartPos, currentChunkRowCount, currentChunkSizeInBytes));
                    currentChunkStartPos = totalConsumedBytes;
                    currentChunkRowCount = 0;
                }
            }

            pipeReader.AdvanceTo(sequenceReader.Position, readResult.Buffer.End);

        } while (!readResult.IsCompleted);

        return chunks.ToArray();
    }

    private static bool ChunkLimitAchived(long sizeInBytes, int lineCount)
        => sizeInBytes >= Settings.ChunkMaxSizeInBytes || lineCount == Settings.ArrayPoolLengthLimit;

}
