﻿namespace Sorter.Common;

internal static class TempFilePathFactory
{
    private const string ChunkFileExt = ".txt";
    private const string ChunkInfoFileExt = ".info.json";
    private static readonly string _workingDirPath = Path.Combine(Directory.GetCurrentDirectory(), "workingDir");

    static TempFilePathFactory()
    {
        if (GlobalSettings.CleanWorkingDirBefore && Directory.Exists(_workingDirPath))
        {
            Directory.Delete(_workingDirPath, true);
        }
        Directory.CreateDirectory(_workingDirPath);
    }

    public static string CreateChunkFilePath(string prefix, int chunkId)
        => Path.Combine(_workingDirPath, $"{prefix}_{chunkId}{ChunkFileExt}");

    public static string CreateChunkInfoFilePath(string prefix, int chunkId)
        => Path.Combine(_workingDirPath, $"{prefix}_{chunkId}{ChunkInfoFileExt}");

    public static string[] GetChunkInfoFilePaths(string prefix)
        => Directory.EnumerateFiles(_workingDirPath, $"{prefix}_*{ChunkInfoFileExt}").ToArray();
}
