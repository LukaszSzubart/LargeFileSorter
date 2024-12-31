using System.Globalization;
using Tools;

namespace Sorter;

public class GlobalSettings
{
    public static int ThreadLimit => 7;
    public static long MemoryLimit => BinarySize.InBytes.GB;

    public static int ArrayPoolLengthLimit => 1024 * 1024;

    public static int ChunkMaxSizeInBytes => 50 * BinarySize.InBytes.MB;

    public static System.StringComparison StringComparison => System.StringComparison.OrdinalIgnoreCase;

    public static System.Text.Encoding Encoding => System.Text.Encoding.ASCII;
    public static string NewLine => "\n";

    public static CultureInfo CultureInfo => CultureInfo.InvariantCulture;
}
