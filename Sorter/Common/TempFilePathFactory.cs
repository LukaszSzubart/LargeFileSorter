namespace Sorter.Common;

internal static class TempFilePathFactory
{
    private const string ChunkFileExt = ".txt";
    private const string ChunkInfoFileExt = ".info.json";
    private static readonly string _workingDirPath = @"C:\code\LargeFileSorter\Sorter\bin\Debug\net9.0\workingDir";// Path.Combine(Directory.GetCurrentDirectory(), "workingDir");

    static TempFilePathFactory()
    {
        if (GlobalSettings.CleanWorkingDirBefore && Directory.Exists(_workingDirPath))
        {
            Directory.Delete(_workingDirPath, true);
        }
        Directory.CreateDirectory(_workingDirPath);
    }

    public static string CreateChunkFilePath(string phaseId, int chunkId)
        => Path.Combine(_workingDirPath, $"{phaseId}_{chunkId}{ChunkFileExt}");
    public static string CreateChunkInfoFilePath(string phaseId, int chunkId)
        => Path.Combine(_workingDirPath, $"{phaseId}_{chunkId}{ChunkInfoFileExt}");

    public static string[] GetChunkInfoFilePaths(string phaseId)
        => Directory.EnumerateFiles(_workingDirPath, $"*{ChunkInfoFileExt}").ToArray();
}
