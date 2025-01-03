using BenchmarkDotNet.Running;
using Benchmarks;

var summary = BenchmarkRunner.Run<PersistentChunkLazyReaderBenchmark>();

//var rootDir = "C:\\data\\tmp\\benchmark\\source.txt";
//var f = new Fixture();
//var rows = f.CreateMany<Row>(10000).ToArray();

//using var outStream = File.OpenWrite(rootDir);
//JsonSerializer.Serialize(outStream,rows);


//var tmp = new IntermediateChunkWriterBenchmark();
//tmp.Setup();
//await tmp.Pipe();