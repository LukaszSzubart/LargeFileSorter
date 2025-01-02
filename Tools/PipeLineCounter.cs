using System.Buffers;
using System.IO.Pipelines;

namespace Tools;

public static class PipeLineCounter
{
    private const char LF = '\n';

    public static async Task<long> CountWithTryAdvanceTo(Stream stream)
    {
        ArgumentNullException.ThrowIfNull(stream);

        var options = new StreamPipeReaderOptions(bufferSize: BinarySize.InBytes.MB, minimumReadSize: BinarySize.InBytes.MB);
        var reader = PipeReader.Create(stream, options);

        long lineCount = 0L;

        ReadResult result;
        var position = 0L;
        do
        {
            result = await reader.ReadAsync();
            
            var sequenceReader = new SequenceReader<byte>(result.Buffer);
            
            while(sequenceReader.TryAdvanceTo((byte)LF, true))
            {
                position += sequenceReader.Consumed;
                lineCount++;
            }

            reader.AdvanceTo(sequenceReader.Position, result.Buffer.End);

        } while (!result.IsCompleted);

        return lineCount;
    }

    public static async Task<long> CountWithTryReadto(Stream stream)
    {
        ArgumentNullException.ThrowIfNull(stream);

        var options = new StreamPipeReaderOptions(bufferSize: BinarySize.InBytes.MB, minimumReadSize: BinarySize.InBytes.MB);
        var reader = PipeReader.Create(stream, options);

        long lineCount = 0L;

        ReadResult result;
        var position = 0L;
        do
        {
            result = await reader.ReadAsync();

            var sequenceReader = new SequenceReader<byte>(result.Buffer);

            while (sequenceReader.TryReadTo(out ReadOnlySpan<byte> bytes, (byte)LF, true))
            {
                position += bytes.Length;
                lineCount++;
            }

            reader.AdvanceTo(sequenceReader.Position, result.Buffer.End);

        } while (!result.IsCompleted);


        return lineCount;
    }
}
