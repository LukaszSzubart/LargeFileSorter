using System.Buffers;

namespace Sorter.Common;
internal class IntermidiateChunkStreamWriter
{
    private static ReadOnlyMemory<byte> NumberStringSeparatorBytes { get; } = GlobalSettings.Encoding.GetBytes(". ").AsMemory();
    private static ReadOnlyMemory<byte> NewLineBytes { get; } = GlobalSettings.Encoding.GetBytes(GlobalSettings.NewLine).AsMemory();

    public static Task Write(IntermediateChunkInfo chunkInfo, IReadOnlyList<Row> rows)
    {
        using var stream = OpenWriteStream(chunkInfo);

        for (int i = 0; i < chunkInfo.RowCount; i++)
        {
            WriteRow(stream, rows[i]);
        }

        return Task.CompletedTask;
    }

    private static FileStream OpenWriteStream(IntermediateChunkInfo chunkInfo)
    {
        var options = new FileStreamOptions
        {
            Access = FileAccess.Write,
            Mode = FileMode.Create,
            PreallocationSize = chunkInfo.SizeInBytes,
            Options = FileOptions.None,
            Share = FileShare.None
        };
        return new FileStream(chunkInfo.FilePath, options);
    }

    private static void WriteRow(FileStream stream, Row row)
    {
        Span<byte> buffer = stackalloc byte[512];
        IMemoryOwner<byte>? memory = null;
        var requiredSize = CalculateBufferSize(row);
        if (requiredSize > 512)
        {
            memory = MemoryPool<byte>.Shared.Rent(requiredSize);
            buffer = memory.Memory.Span;
        }

        if (!row.Number.TryFormat(buffer, out var numberBytesWrote, provider: GlobalSettings.CultureInfo))
            throw new Exception("failed to write Number into file");

        stream.Write(buffer[..numberBytesWrote]);
        stream.Write(NumberStringSeparatorBytes.Span);

        var stringBytesWrote = GlobalSettings.Encoding.GetBytes(row.String, buffer);
        stream.Write(buffer[..stringBytesWrote]);

        stream.Write(NewLineBytes.Span);

        memory?.Dispose();
    }


    private static int CalculateBufferSize(Row row)
        => 20 //ulong max value character count
        + 4 // space + dot + newLine (max 2 chars)
        + GlobalSettings.Encoding.GetByteCount(row.String);
}
