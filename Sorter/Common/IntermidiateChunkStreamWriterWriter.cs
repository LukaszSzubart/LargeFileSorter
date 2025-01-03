namespace Sorter.Common;

internal class IntermidiateChunkStreamWriterWriter
{
    public static Task Write(PersistentChunkInfo chunkInfo, Row[] rows)
    {
        using var stream = OpenWriteStream(chunkInfo);
        using var writer = new StreamWriter(stream, GlobalSettings.Encoding, 1024 * 1024);
        writer.AutoFlush = true;
        writer.NewLine = GlobalSettings.NewLine;

        for (int i = 0; i < chunkInfo.RowCount; i++)
        {
            WriteRow(writer, rows[i]);
        }

        return Task.CompletedTask;
    }

    private static FileStream OpenWriteStream(PersistentChunkInfo chunkInfo)
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

    private static void WriteRow(StreamWriter writer, Row row)
    {
        writer.Write(row.Number);
        writer.Write('.');
        writer.Write(' ');
        writer.WriteLine(row.String);
    }
}
