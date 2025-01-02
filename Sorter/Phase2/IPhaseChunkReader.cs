using Sorter.Common;

namespace Sorter.Phase2;
internal interface IPhaseChunkReader
{
    bool Completed { get; }
    Row Row { get; }

    ValueTask<bool> Reload();
}