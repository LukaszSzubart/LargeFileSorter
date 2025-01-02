using Sorter.Common;
using System.Buffers;
using System.Diagnostics;
using System.IO.Pipelines;
using System.Threading.Channels;

namespace Sorter.Phase2;
internal class Phase2ChannelChunkReader : IPhaseChunkReader
{
    private const int BufferSize = 256;
    private readonly IntermediateChunkInfo _chunkInfo;
    private readonly Channel<Row> _chunkChannel;

    public Phase2ChannelChunkReader(IntermediateChunkInfo chunkInfo)
    {
        _chunkInfo = chunkInfo;
        _chunkChannel = Channel.CreateBounded<Row>(new BoundedChannelOptions(GlobalSettings.IntermediateChunkReadChannelCapacity)
        {
            SingleReader = true,
            SingleWriter = true,
            FullMode = BoundedChannelFullMode.Wait
        });
        _ = Task.Run(StartFileRead);
        Row = Row.Empty;
    }

    public Row Row { get; private set; }
    public bool Completed { get; private set; }

    public ValueTask<bool> Reload()
    {
        if (Completed)
            return ValueTask.FromResult(false);

        if (_chunkChannel.Reader.TryRead(out var row))
        {
            Row = row;
            return ValueTask.FromResult(false);
        }

        return new ValueTask<bool>(ReloadAsync());
    }

    private async Task<bool> ReloadAsync()
    {
        if (await _chunkChannel.Reader.WaitToReadAsync())
        {
            if (_chunkChannel.Reader.TryRead(out var row))
            {
                Row = row;
                return false;
            }
            throw new Exception("something went wrong");
        }
        Row = Row.Empty;
        Completed = true;
        return true;
    }

    private async Task StartFileRead()
    {
        try
        {
            await using var stream = OpenFile(_chunkInfo);
            var reader = PipeReader.Create(stream);
            var arrayPool = ArrayPool<Row>.Shared;
            var rowsBuffer = arrayPool.Rent(BufferSize);

            var totalRows = 0;
            var readCompleted = false;
            ReadResult readResult;
            do
            {
                readResult = await reader.ReadAsync();
                readCompleted = readResult.IsCompleted;

                var delimiterBytes = GlobalSettings.NewLineBytes.Span;
                var sequenceReader = new SequenceReader<byte>(readResult.Buffer);
                var rowsRead = 0;

                while (rowsRead < BufferSize && sequenceReader.TryReadTo(out ReadOnlySpan<byte> bytes, delimiterBytes, true))
                {
                    var row = RowDeserializer.Deserialize(bytes);
                    rowsBuffer[rowsRead++] = row;
                }

                totalRows += rowsRead;

                reader.AdvanceTo(sequenceReader.Position, sequenceReader.Sequence.End);

                await WriteToChannel(rowsBuffer, rowsRead);

            } while (!readCompleted);

            Debug.Assert(totalRows == _chunkInfo.RowCount);

            _chunkChannel.Writer.Complete();
            arrayPool.Return(rowsBuffer);
        }
        catch (Exception ex)
        {
            _chunkChannel.Writer.Complete(ex);
        }


    }

    private async ValueTask WriteToChannel(Row[] rowsBuffer, int count)
    {
        for (int i = 0; i < count; i++)
        {
            await _chunkChannel.Writer.WriteAsync(rowsBuffer[i]);
        }
    }
    private static FileStream OpenFile(IntermediateChunkInfo chunkInfo)
    {
        var fsOptions = new FileStreamOptions
        {
            Access = FileAccess.Read,
            Mode = FileMode.Open,
            Share = FileShare.Read,
            Options = FileOptions.Asynchronous
        };

        return new FileStream(chunkInfo.FilePath, fsOptions);
    }
}
