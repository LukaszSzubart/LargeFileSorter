namespace Sorter.Common;

internal static class TempFilePathFactory
{
    private static readonly string _workingDirPath = Path.Combine(Directory.GetCurrentDirectory(), "workingDir");

    static TempFilePathFactory()
    {
        Directory.Delete(_workingDirPath, true);
        Directory.CreateDirectory(_workingDirPath);
    }

    public static string Create(int chunkId)
        => Path.Combine(_workingDirPath, $"{chunkId}.txt");
}
