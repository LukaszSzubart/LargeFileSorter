using Common;

namespace TestFileGenerator
{
    internal class Settings
    {
        const long defaultSizeInBytes = 100 * BinarySize.InBytes.MB;
        //const long defaultSizeInBytes = 1 * BinarySize.InBytes.GB;

        private Settings() { }

        public required long ExpectedOutFileSizeInBytes { get; init; }

        public required string OutFilePath { get; init; }

        public static Settings CreateFrom(string[] args)
        {
            var expectedOutFileSizeInBytes = args.Length > 0 && int.TryParse(args[0], out var sizeFromArgs)
            ? sizeFromArgs * BinarySize.InBytes.MB
            : defaultSizeInBytes;
            var expectedOutFileSizeInMB = expectedOutFileSizeInBytes / BinarySize.InBytes.MB;

            string OutFilePath = $".\\{expectedOutFileSizeInMB}mb.txt";

            return new Settings
            {
                OutFilePath = OutFilePath,
                ExpectedOutFileSizeInBytes = expectedOutFileSizeInBytes
            };
        }
    }
}
