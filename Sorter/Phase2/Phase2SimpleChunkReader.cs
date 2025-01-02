using Sorter.Common;
using System.Buffers;
using System.IO.Pipelines;

namespace Sorter.Phase2;
internal sealed class Phase2SimpleChunkReader : IDisposable, IPhaseChunkReader
{
    private const int BufferSize = 256;
    private readonly Stream _chunkFileStream;
    private readonly PipeReader _chunkReader;
    private readonly IntermediateChunkInfo _chunkInfo;
    private readonly ArrayPool<Row> _arrayPool;
    private Row[] _rowBuffer;
    private int _bufferIndex = 0;
    private int _rowsRead = 0;


    public Phase2SimpleChunkReader(IntermediateChunkInfo chunkInfo)
    {
        _chunkInfo = chunkInfo;
        _arrayPool = ArrayPool<Row>.Shared;
        _rowBuffer = _arrayPool.Rent(BufferSize);


        _chunkFileStream = OpenFile(_chunkInfo);
        _chunkReader = PipeReader.Create(_chunkFileStream);
    }

    public Row Row => _rowBuffer[_bufferIndex];
    public bool Completed { get; private set; }



    public ValueTask<bool> Reload()
    {
        _bufferIndex++;
        if(_bufferIndex < _rowsRead)
        {
            return ValueTask.FromResult(false);
        }

        if (Completed)
            return ValueTask.FromResult(true);

        return new ValueTask<bool>(LoadBuffer());
    }

    public void Dispose()
    {
        _chunkFileStream?.Dispose();
        var array = _rowBuffer;
        _rowBuffer = null;
        _arrayPool?.Return(array);
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
        _chunkReader.AdvanceTo(sequenceReader.Position, sequenceReader.Sequence.End);

        Completed = readResult.IsCompleted;

        return Completed && rowsRead == 0;
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
