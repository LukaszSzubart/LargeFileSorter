using Sorter.Phase0;

const string dir = "C:\\code\\LargeFileSorter\\TestFileGenerator\\bin\\Release\\net9.0\\";

const string path = dir + "1024mb.txt";
//const string path = dir + "100mb.txt";





var chunks = await ChunkFactory.Create(new Sorter.RunInfo(path));

Console.WriteLine("end");


//await Measure(stream => new ValueTask<long>(StreamLineCounter.Count(stream)));
//await Measure(async stream => await PipeLineCounter.Count(stream));

//async Task Measure(Func<Stream, ValueTask<long>> countLines)
//{
//    using var stream = File.OpenRead(path);
//    var stopwatch = Stopwatch.StartNew();
//    var lineCount = await countLines(stream);
//    stopwatch.Stop();
//    Console.WriteLine($"Line count= {lineCount}. Time= {stopwatch.Elapsed}");
//}
