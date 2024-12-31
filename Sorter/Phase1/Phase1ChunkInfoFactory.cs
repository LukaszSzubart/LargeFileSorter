using Sorter.Common;
using System.Buffers;
using System.IO.Pipelines;

namespace Sorter.Phase1;

internal static class Phase1ChunkInfoFactory
{
    private const char LF = '\n';

    public static async Task<IReadOnlyList<Phase1ChunkInfo>> Create(RunInfo runInfo)
    {
        ArgumentNullException.ThrowIfNull(runInfo);
        await using var inputFileStream = File.OpenRead(runInfo.InputFilePath);
        var pipeReader = PipeReader.Create(inputFileStream);

        List<Phase1ChunkInfo> chunks = new List<Phase1ChunkInfo>(512);

        ReadResult readResult;
        var chunktContext = new ChunkContext();
        do
        {
            readResult = await pipeReader.ReadAsync();

            var sequenceReader = new SequenceReader<byte>(readResult.Buffer);

            while (sequenceReader.TryReadTo(out ReadOnlySpan<byte> consumed, (byte)LF, true))
            {
                chunktContext.Update(consumed.Length);
                if (chunktContext.LimitAchived)
                {
                    var chunkInfo = CreateInfo(runInfo, chunktContext);
                    chunks.Add(chunkInfo);

                    chunktContext.NextChunk();
                }
            }

            pipeReader.AdvanceTo(sequenceReader.Position, sequenceReader.Sequence.End);

        } while (!readResult.IsCompleted);

        return chunks;
    }

    private static Phase1ChunkInfo CreateInfo(RunInfo runInfo, ChunkContext ctx)
    {
        var sourceInfo = new SourceChunkInfo(runInfo.InputFilePath, ctx.StartPos, ctx.RowCount, ctx.SizeInBytes);
        var destPath = TempFilePathFactory.Create(ctx.Id);
        var destInfo = new IntermediateChunkInfo(destPath, ctx.RowCount, ctx.SizeInBytes);
        return new(sourceInfo, destInfo);
    }


    private struct ChunkContext
    {
        public ChunkContext()
        {
            _totalConsumedBytes = 0;
            Id = 0;
            StartPos = 0;
            RowCount = 0;
            SizeInBytes = 0;
        }

        private long _totalConsumedBytes;
        public int Id { get; private set; }
        public long StartPos { get; private set; }
        public int RowCount { get; private set; }
        public long SizeInBytes { get; private set; }

        public readonly bool LimitAchived
            => SizeInBytes >= GlobalSettings.ChunkMaxSizeInBytes || RowCount == GlobalSettings.ArrayPoolLengthLimit;

        public void Update(long consumedBytes)
        {
            _totalConsumedBytes += consumedBytes;
            RowCount++;
            SizeInBytes = _totalConsumedBytes - StartPos;
        }

        public void NextChunk()
        {
            StartPos = _totalConsumedBytes;
            RowCount = 0;
            SizeInBytes = 0;
            Id++;
        }
    }
}
