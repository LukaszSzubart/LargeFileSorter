namespace Sorter.Common;

internal class RowDeserializer
{
    public static Row Deserialize(in ReadOnlySpan<byte> lineBytes)
    {
        try
        {
            var dotIndex = lineBytes.IndexOf((byte)'.');
            var numer = ulong.Parse(lineBytes.Slice(0, dotIndex), provider: GlobalSettings.CultureInfo);
            var str = GlobalSettings.Encoding.GetString(lineBytes.Slice(dotIndex + 2));
            return new Row(numer, str);
        }
        catch (Exception ex)
        {
            throw;
        }
    }
}
