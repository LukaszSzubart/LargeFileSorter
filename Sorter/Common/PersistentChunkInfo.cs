namespace Sorter.Common;


internal record PersistentChunkInfo (int Id, string FilePath, long RowCount, long SizeInBytes);
