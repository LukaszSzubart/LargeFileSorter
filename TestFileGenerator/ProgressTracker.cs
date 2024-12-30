namespace TestFileGenerator;

internal class ProgressTracker(long expectedSizeInBytes)
{
    public long BytesWritten { get; set; }

    public long ExpectedSizeInBytes => expectedSizeInBytes;

    public long Progress => (long)((double)BytesWritten / ExpectedSizeInBytes * 100);

    public bool Finished => BytesWritten >= expectedSizeInBytes;

    public static ProgressTracker CreateFrom(Settings settings) => new ProgressTracker(settings.ExpectedOutFileSizeInBytes);
}