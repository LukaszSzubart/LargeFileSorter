﻿using Common;
using System.Text;

namespace TestFileGenerator
{
    internal sealed class OutFile : IDisposable
    {
        private OutFile(string path, long expectedSize)
        {
            var outFileOptions = new FileStreamOptions
            {
                PreallocationSize = expectedSize,
                Access = FileAccess.Write,
                BufferSize = 10 * (int)BinarySize.InBytes.MB,
                Mode = FileMode.Create,
                Options = FileOptions.WriteThrough,
                Share = FileShare.None
            };
            Writer = new StreamWriter(path, Encoding.ASCII, outFileOptions);
        }

        public StreamWriter Writer { get; }

        public void Dispose()
        {
            Writer?.Dispose();
        }

        public static OutFile CreateFrom(Settings settings)
            => new(settings.OutFilePath, settings.ExpectedOutFileSizeInBytes);
    }
}