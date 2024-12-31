namespace Sorter.Phase1;

internal interface ILineFactory
{
    ILine Create(ref ReadOnlySpan<byte> lineBytes);
}
