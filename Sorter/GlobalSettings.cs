using System.Globalization;
using Tools;

namespace Sorter;

public static class GlobalSettings
{
    public static int ThreadLimit => 1;
    public static long MemoryLimit => BinarySize.InBytes.GB;

    public static int ArrayPoolLengthLimit => 1024 * 1024;

    public static int ChunkMaxSizeInBytes => 50 * BinarySize.InBytes.MB;

    public static System.StringComparison StringComparison => System.StringComparison.OrdinalIgnoreCase;

    public static System.Text.Encoding Encoding => System.Text.Encoding.ASCII;

    public static string NewLine => "\n";

    public static ReadOnlyMemory<byte> NewLineBytes { get; } = Encoding.GetBytes(NewLine).AsMemory();

    public static CultureInfo CultureInfo => CultureInfo.InvariantCulture;

    public static bool DumpIntermidieteChunkDefinition => true;

    public static bool CleanWorkingDirBefore => false;
}
