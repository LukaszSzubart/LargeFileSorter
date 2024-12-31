using System.Runtime.CompilerServices;

namespace Sorter.Common;

internal sealed record Row
{
    public Row(ulong number, string @string)
    {
        String = @string;
        Number = number;
    }

    public string String { get; }
    public ulong Number { get; }

    public static IComparer<Row> Comparer { get; } = new RowComparer();

    private class RowComparer : IComparer<Row>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Compare(Row? x, Row? y)
        {
            const int XIsLess = -1;
            const int XIsGreater = 1;
            const int AreEqual = 0;

            var xIsNull = x is null;
            var yIsNull = y is null;

            if (!xIsNull && yIsNull)
                return XIsLess;

            if (xIsNull && yIsNull)
                return AreEqual;

            if (xIsNull && !yIsNull)
                return XIsGreater;

            var stringComparisonResult = string.Compare(x!.String, y!.String, GlobalSettings.StringComparison);

            if (stringComparisonResult != AreEqual)
                return stringComparisonResult;

            return x!.Number.CompareTo(y!.Number);
        }
    }
}



