using Sorter.Common;
using System.Buffers;
using System.IO.Pipelines;
using Tools;

namespace Sorter.Phase1;

internal static class SourceChunkReader
{
    public async static Task<SourceChunk> Read(SourceChunkInfo chunkInfo)
    {
        await using var stream = OpenFile(chunkInfo);
        stream.Position = chunkInfo.StartPos;
        var reader = PipeReader.Create(stream);
        var arrayPool = ArrayPool<Row>.Shared;
        var lineBuffer = arrayPool.Rent(GlobalSettings.ArrayPoolLengthLimit);

        long index = 0L;
        ReadResult result;
        do
        {
            result = await reader.ReadAsync();

            var delimiterBytes = GlobalSettings.NewLineBytes.Span;
            var sequenceReader = new SequenceReader<byte>(result.Buffer);

            while (index < chunkInfo.LineCount && sequenceReader.TryReadTo(out ReadOnlySpan<byte> bytes, delimiterBytes, true))
            {
                var line = RowDeserializer.Deserialize(bytes);
                lineBuffer[index] = line;
                index++;
            }

            reader.AdvanceTo(sequenceReader.Position, result.Buffer.End);

        } while (index < chunkInfo.LineCount);

        return new SourceChunk(chunkInfo, lineBuffer, arrayPool);
    }


    private static FileStream OpenFile(SourceChunkInfo chunkInfo)
    {
        var fsOptions = new FileStreamOptions
        {
            Access = FileAccess.Read,
            Mode = FileMode.Open,
            Share = FileShare.Read
        };

        return new FileStream(chunkInfo.FilePath, fsOptions);
    }
}
