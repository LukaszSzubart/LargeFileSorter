using Tools;

namespace Sorter;

public class Settings
{
    public static int ThreadLimit => 7;
    public static long MemoryLimit => BinarySize.InBytes.GB;

    public static int ArrayPoolLengthLimit => 1024 * 1024;

    public static int ChunkMaxSizeInBytes => 50 * BinarySize.InBytes.MB;
}
