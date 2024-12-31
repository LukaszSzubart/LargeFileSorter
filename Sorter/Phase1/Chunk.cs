using Sorter.Phase0;
using System.Buffers;

namespace Sorter.Phase1;

internal sealed class Chunk : IDisposable
{
    private readonly ArrayPool<ILine> _arrayPool;
    public Chunk(ChunkInfo info)
    {
        Info = info;
        _arrayPool = ArrayPool<ILine>.Shared;
        Lines = _arrayPool.Rent(Settings.ArrayPoolLengthLimit);
    }

    public ChunkInfo Info { get; }

    public ILine[] Lines { get; private set; }

    public void Dispose()
    {
        var lines = Lines;
        Lines = [];
        _arrayPool?.Return(lines);
    }
}
