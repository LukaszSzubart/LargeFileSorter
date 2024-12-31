using System.Buffers;
using System.IO.Pipelines;

namespace Sorter.Common;

internal static class IntermidiateChunkPipeWriter
{

    private static ReadOnlyMemory<byte> NumberStringSeparatorBytes { get; } = GlobalSettings.Encoding.GetBytes(". ").AsMemory();
    private static ReadOnlyMemory<byte> NewLineBytes { get; } = GlobalSettings.Encoding.GetBytes(GlobalSettings.NewLine).AsMemory();

    public static async Task Write(IntermediateChunkInfo chunkInfo, Row[] rows)
    {
        await using var stream = OpenWrite(chunkInfo);
        var writer = CreatePipeWriter(stream);

        for (int i = 0; i < chunkInfo.RowCount; i++)
        {
            await WriteRow(writer, rows[i]);
        }
    }

    private static FileStream OpenWrite(IntermediateChunkInfo chunkInfo) 
    {
        var options = new FileStreamOptions
        {
            Access = FileAccess.Write,
            Mode = FileMode.Create,
            PreallocationSize = chunkInfo.SizeInBytes,
            Options = FileOptions.Asynchronous,
            Share = FileShare.Read
        };
        return new FileStream(chunkInfo.FilePath, options);
    }

    private static PipeWriter CreatePipeWriter(Stream stream)
    {
        var options = new StreamPipeWriterOptions();
        return PipeWriter.Create(stream, options);
    }

    private static ValueTask<bool> WriteRow(PipeWriter writer, Row row)
    {
        // 20 is length of ulong.MaxValue=18446744073709551615
        Span<char> numberBuffer = stackalloc char[20];
        if (!row.Number.TryFormat(numberBuffer, out var charsWritten, provider: GlobalSettings.CultureInfo))
            throw new Exception();
        var numberDestSpan = writer.GetSpan(GlobalSettings.Encoding.GetByteCount(numberBuffer[..charsWritten]));
        var numberBytesWrote = GlobalSettings.Encoding.GetBytes(numberBuffer[..charsWritten], numberDestSpan);
        writer.Advance(numberBytesWrote);

        writer.Write(NumberStringSeparatorBytes.Span);

        var stringDestSpan = writer.GetSpan(GlobalSettings.Encoding.GetByteCount(row.String));
        var stringBytesWrote = GlobalSettings.Encoding.GetBytes(row.String, stringDestSpan);
        writer.Advance(stringBytesWrote);

        writer.Write(NewLineBytes.Span);

       return Flush(writer);
    }

    private static ValueTask<bool> Flush(PipeWriter writer)
    {
        bool GetResult(FlushResult flush)
            // tell the calling code whether any more messages
            // should be written
            => !(flush.IsCanceled || flush.IsCompleted);

        async ValueTask<bool> Awaited(ValueTask<FlushResult> incomplete)
            => GetResult(await incomplete);

        // apply back-pressure etc
        var flushTask = writer.FlushAsync();

        return flushTask.IsCompletedSuccessfully
            ? new ValueTask<bool>(GetResult(flushTask.Result))
            : Awaited(flushTask);
    }
}
