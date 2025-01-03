using Sorter.Common;
using System.Buffers;
using System.IO.Pipelines;

namespace Sorter.Phase1;

internal static class VirtualChunkReader
{
    public async static Task<ChunkData> Read(VirtualChunkInfo chunkInfo)
    {
        await using var stream = OpenFile(chunkInfo);
        stream.Position = chunkInfo.Offset;
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

            while (index < chunkInfo.RowCount && sequenceReader.TryReadTo(out ReadOnlySpan<byte> bytes, delimiterBytes, true))
            {
                var line = RowDeserializer.Deserialize(bytes);
                lineBuffer[index] = line;
                index++;
            }

            reader.AdvanceTo(sequenceReader.Position, result.Buffer.End);

        } while (index < chunkInfo.RowCount);

        return new ChunkData(lineBuffer, arrayPool);
    }


    private static FileStream OpenFile(VirtualChunkInfo chunkInfo)
    {
        var fsOptions = new FileStreamOptions
        {
            Access = FileAccess.Read,
            Mode = FileMode.Open,
            Share = FileShare.Read,
            Options = FileOptions.Asynchronous
        };

        return new FileStream(chunkInfo.FilePath, fsOptions);
    }
}
