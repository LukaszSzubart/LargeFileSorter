using Sorter.Common;
using System.Buffers;
using System.IO.Pipelines;

namespace Sorter.Phase1;

internal static class Phase1ChunkInfoFactory
{
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

            var delimiterBytes = GlobalSettings.NewLineBytes.Span;
            var sequenceReader = new SequenceReader<byte>(readResult.Buffer);

            while (sequenceReader.TryReadTo(out ReadOnlySpan<byte> consumed, delimiterBytes, true))
            {
                chunktContext.Update(consumed.Length + delimiterBytes.Length);
                if (chunktContext.LimitAchived)
                {
                    var chunkInfo = CreateInfo(runInfo, chunktContext);
                    chunks.Add(chunkInfo);

                    chunktContext.NextChunk();
                }
            }

            pipeReader.AdvanceTo(sequenceReader.Position, sequenceReader.Sequence.End);

        } while (!readResult.IsCompleted);

        if(chunktContext.ConsumedBytes > 0)
        {
            var chunkInfo = CreateInfo(runInfo, chunktContext);
            chunks.Add(chunkInfo);
        }

        return chunks;
    }

    private static Phase1ChunkInfo CreateInfo(RunInfo runInfo, ChunkContext ctx)
    {
        var sourceInfo = new SourceChunkInfo(ctx.Id, runInfo.InputFilePath, ctx.StartPos, ctx.RowCount, ctx.ConsumedBytes);
        var destPath = TempFilePathFactory.CreateChunkFilePath("1", ctx.Id);
        var destInfo = new IntermediateChunkInfo(ctx.Id, destPath, ctx.RowCount, ctx.ConsumedBytes);
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
            ConsumedBytes = 0;
        }

        private long _totalConsumedBytes;
        public int Id { get; private set; }
        public long StartPos { get; private set; }
        public int RowCount { get; private set; }
        public long ConsumedBytes { get; private set; }

        public readonly bool LimitAchived
            => ConsumedBytes >= GlobalSettings.ChunkMaxSizeInBytes || RowCount == GlobalSettings.ArrayPoolLengthLimit;

        public void Update(long consumedBytes)
        {
            _totalConsumedBytes += consumedBytes;
            RowCount++;
            ConsumedBytes = _totalConsumedBytes - StartPos;
        }

        public void NextChunk()
        {
            StartPos = _totalConsumedBytes;
            RowCount = 0;
            ConsumedBytes = 0;
            Id++;
        }
    }
}
