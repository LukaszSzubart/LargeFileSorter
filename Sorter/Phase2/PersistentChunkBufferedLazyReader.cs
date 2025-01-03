using Sorter.Common;
using System.Buffers;
using System.Diagnostics;
using System.IO.Pipelines;

namespace Sorter.Phase2;
internal sealed class PersistentChunkBufferedLazyReader : IAsyncEnumerator<Row>
{
    private const int BufferSize = 256;
    private Stream _chunkFileStream;
    private readonly PipeReader _chunkReader;
    private readonly PersistentChunkInfo _chunkInfo;
    private ArrayPool<Row> _arrayPool;
    private Row[] _rowBuffer;
    private int _bufferIndex = 0;
    private int _rowsRead = 0;
    private bool _completed = false;
    private bool _disposed = false;
    private int _totalRowsRead = 0;

    public PersistentChunkBufferedLazyReader(PersistentChunkInfo chunkInfo)
    {
        _chunkInfo = chunkInfo;
        _arrayPool = ArrayPool<Row>.Shared;
        _rowBuffer = _arrayPool.Rent(BufferSize);
        _chunkFileStream = OpenFile(_chunkInfo);
        _chunkReader = PipeReader.Create(_chunkFileStream);
    }

    public Row Current => _rowBuffer[_bufferIndex];

    private static FileStream OpenFile(PersistentChunkInfo chunkInfo)
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

    public ValueTask<bool> MoveNextAsync()
    {
        _bufferIndex++;
        if(_bufferIndex < _rowsRead)
        {
            return ValueTask.FromResult(true);
        }

        if (_completed)
        {
            if (_disposed)
            {
                return ValueTask.FromResult(false);
            }
            return DisposeAndReturnFalse();
        }

        return new ValueTask<bool>(LoadBuffer());
    }

    public ValueTask DisposeAsync()
    {
        if (_disposed)
        {
            return ValueTask.CompletedTask;
        }
        var array = _rowBuffer;
        _rowBuffer = [];
        _arrayPool.Return(array);

        var stream = _chunkFileStream;
        _chunkFileStream = Stream.Null;

        _disposed = true;
        return stream.DisposeAsync();
    }


    private async ValueTask<bool> DisposeAndReturnFalse()
    {
        await DisposeAsync();
        return false;
    }

    private async Task<bool> LoadBuffer()
    {
        var readResult = await _chunkReader.ReadAsync();

        var delimiterBytes = GlobalSettings.NewLineBytes.Span;
        var sequenceReader = new SequenceReader<byte>(readResult.Buffer);
        var rowsRead = 0;

        while (rowsRead < BufferSize && sequenceReader.TryReadTo(out ReadOnlySpan<byte> bytes, delimiterBytes, true))
        {
            var row = RowDeserializer.Deserialize(bytes);
            _rowBuffer[rowsRead++] = row;
        }
        _bufferIndex = 0;
        _rowsRead = rowsRead;
        _totalRowsRead += rowsRead;
        _chunkReader.AdvanceTo(sequenceReader.Position, sequenceReader.Sequence.End);

        _completed = readResult.IsCompleted;

        Debug.Assert(!_completed || _completed && _totalRowsRead == _chunkInfo.RowCount);

        return rowsRead > 0;
    }
}
