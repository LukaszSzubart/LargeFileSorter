using System.Text.Json;

namespace Sorter.Common;
internal static class IntermediateChunkInfoDumper
{
    public static async Task Dump(string phaseId, IntermediateChunkInfo chunkInfo)
    {
        await using var outStream = OpenWrite(phaseId, chunkInfo.Id);
        await JsonSerializer.SerializeAsync(outStream, chunkInfo, new JsonSerializerOptions
        {
            WriteIndented = true
        });
    }

    public static async Task<IReadOnlyList<IntermediateChunkInfo>> ReadDump(string phaseId)
    {
        var paths = TempFilePathFactory.GetChunkInfoFilePaths(phaseId);
        var infos = new List<IntermediateChunkInfo>(paths.Length);

        foreach (var path in paths)
        {
            await using var inStream = OpenRead(path);

            var info = await JsonSerializer.DeserializeAsync<IntermediateChunkInfo>(inStream);
            infos.Add(info);
        }
        return infos;
    }

    private static FileStream OpenWrite(string phaseId, int chunkId)
    {
        var path = TempFilePathFactory.CreateChunkInfoFilePath(phaseId, chunkId);

        return new FileStream(path, new FileStreamOptions
        {
            Access = FileAccess.Write,
            Mode = FileMode.Create,
            Options = FileOptions.WriteThrough | FileOptions.Asynchronous,
            Share = FileShare.None
        });
    }

    private static FileStream OpenRead(string path)
    {
        return new FileStream(path, new FileStreamOptions
        {
            Access = FileAccess.Read,
            Mode = FileMode.Open,
            Options = FileOptions.Asynchronous,
            Share = FileShare.None
        });
    }
}
