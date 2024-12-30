namespace TestFileGenerator;

internal static partial class TestFileWriter
{
    public static void Write(string[] args)
    {
        var settings = Settings.CreateFrom(args);

        var progressTracker = ProgressTracker.CreateFrom(settings);

        using var progressReporter = new ConsoleProgressReporter(TimeSpan.FromSeconds(1), progressTracker);

        progressReporter.ReportStart();

        using var outFile = OutFile.CreateFrom(settings);

        do
        {
            progressTracker.BytesWritten += LineWriter.WriteLine(outFile.Writer);

        } while (!progressTracker.Finished);

        progressReporter.ReportDone();
    }
}
