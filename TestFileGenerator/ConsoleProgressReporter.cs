using Common;
using Timer = System.Timers.Timer;

namespace TestFileGenerator;

internal sealed class ConsoleProgressReporter(TimeSpan interval, ProgressTracker progressTracker) : IDisposable
{
    private readonly Timer _timer = Initialize(interval, progressTracker);

    private static Timer Initialize(TimeSpan interval, ProgressTracker progressTracker)
    {
        var timer = new Timer(interval)
        {
            AutoReset = true
        };
        timer.Elapsed += (s, e) =>
        {
            WriteProgressToConsole(progressTracker);
        };
        timer.Start();
        return timer;
    }

    private static void WriteProgressToConsole(ProgressTracker progressTracker)
    {
        Console.SetCursorPosition(0, 1);
        Console.Write(progressTracker.Progress);
        Console.Write(" %");
    }

    public void ReportStart() =>
        Console.WriteLine($"Generating text file of size: {progressTracker.ExpectedSizeInBytes / BinarySize.InBytes.MB} MB");


    public void ReportDone()
    {
        WriteProgressToConsole(progressTracker);
        Console.WriteLine($"Done.");
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}