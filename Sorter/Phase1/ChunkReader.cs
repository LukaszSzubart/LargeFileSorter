using Sorter.Phase0;
using System.Buffers;
using System.IO.Pipelines;
using Tools;

namespace Sorter.Phase1;

internal static class ChunkReader
{
    public async static Task<Chunk> Read(ChunkInfo chunkInfo)
    {
        var chunk = new Chunk(chunkInfo);

        await using var stream = OpenFile(chunkInfo);

        var reader = PipeReader.Create(stream);

        long lineCount = 0L;

        ReadResult result;
        do
        {
            result = await reader.ReadAsync();

            var sequenceReader = new SequenceReader<byte>(result.Buffer);

            while (sequenceReader.TryReadTo(out ReadOnlySpan<byte> bytes, (byte)ConstChars.LF, true))
            {
                var byteArray = new byte[bytes.Length];
                bytes.CopyTo(byteArray);
                //chunk.Lines[lineCount] = new Line(byteArray);
                lineCount++;

            }

            reader.AdvanceTo(sequenceReader.Position, result.Buffer.End);

        } while (lineCount < chunkInfo.LineCount);

        return chunk;
    }


    private static FileStream OpenFile(ChunkInfo chunkInfo)
    {
        var fsOptions = new FileStreamOptions
        {
            Access = FileAccess.Read,
            Mode = FileMode.Open,
            Share = FileShare.Read
        };

        return new FileStream(chunkInfo.RunInfo.InputFilePath, fsOptions);
    }
}
