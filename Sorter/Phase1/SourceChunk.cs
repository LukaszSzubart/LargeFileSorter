using Sorter.Common;
using System.Buffers;

namespace Sorter.Phase1;

internal sealed class SourceChunk : IDisposable
{
    private readonly ArrayPool<Row> _arrayPool;

    public SourceChunk(SourceChunkInfo info, Row[] lines, ArrayPool<Row> arrayPool)
    {
        Info = info;
        _arrayPool = arrayPool;
        LineBuffer = lines;
    }

    public SourceChunkInfo Info { get; }

    public Row[] LineBuffer { get; private set; }


    public void Dispose()
    {
        var array = LineBuffer;
        LineBuffer = [];
        _arrayPool?.Return(array);
    }
}
