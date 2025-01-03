namespace Sorter.Phase1;

/// <summary>
/// Contains properties which describes set of lines in a given file.
/// </summary>
/// <param name="Id">Unique identifier to distinguish between other chunks</param>
/// <param name="FilePath">Path to source file</param>
/// <param name="Offset">Number of bytes from begining of the file where the chunk starts</param>
/// <param name="RowCount">Number of lines</param>
/// <param name="SizeInBytes"></param>
internal record VirtualChunkInfo(int Id, string FilePath, long Offset, int RowCount, long SizeInBytes);
