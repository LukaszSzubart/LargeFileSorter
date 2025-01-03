using Sorter.Common;
using System.Buffers;

namespace Sorter.Phase1;

internal sealed class ChunkData : IDisposable
{
    private readonly ArrayPool<Row> _arrayPool;

    public ChunkData(Row[] lines, ArrayPool<Row> arrayPool)
    {
        _arrayPool = arrayPool;
        Rows = lines;
    }

    public Row[] Rows { get; private set; }

    public void Dispose()
    {
        var array = Rows;
        Rows = [];
        _arrayPool?.Return(array);
    }
}
