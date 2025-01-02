namespace Benchmarks.TestData;
internal static class TestDataPaths
{
    private static string TestDataDir { get; } = Path.Combine(Directory.GetCurrentDirectory(), "TestData");
    public static string OutDir { get; } = Path.Combine(Directory.GetCurrentDirectory(), "out");
    public static string ChunkSmallSorted { get; } = Path.Combine(TestDataDir, "ChunkSmallSorted.txt");
    public static string RowsSmallJson { get; } = Path.Combine(TestDataDir, "RowsSmall.json");
}
