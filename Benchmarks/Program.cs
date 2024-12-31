using AutoFixture;
using BenchmarkDotNet.Running;
using Benchmarks;
using Sorter.Common;
using System.Text.Json;
using System.Text.Json.Nodes;

var summary = BenchmarkRunner.Run<IntermediateChunkWriterBenchmark>();

//var rootDir = "C:\\data\\tmp\\benchmark\\source.txt";
//var f = new Fixture();
//var rows = f.CreateMany<Row>(10000).ToArray();

//using var outStream = File.OpenWrite(rootDir);
//JsonSerializer.Serialize(outStream,rows);


//var tmp = new IntermediateChunkWriterBenchmark();
//tmp.Setup();
//await tmp.Pipe();