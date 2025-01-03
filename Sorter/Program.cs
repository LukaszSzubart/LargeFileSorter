using Sorter;
using Sorter.Common;
using Sorter.Phase1;
using Sorter.Phase2;
using System.Diagnostics;

const string dir = "C:\\code\\LargeFileSorter\\TestFileGenerator\\bin\\Release\\net9.0\\";

const string path = dir + "1024mb.txt";
//const string path = dir + "100mb.txt";


var runInfo = new Sorter.RunInfo(path);

var sw = Stopwatch.StartNew();

var intermediateChunkInfos = await Phase1Orchestrator.ExecutePhase1(runInfo);

//var intermediateChunkInfos = await IntermediateChunkInfoDumper.ReadDump("1");

var finalResultInfo = await Phase2Orchestrator.ExecutePhase2(intermediateChunkInfos);
sw.Stop();
Console.WriteLine(sw.Elapsed);


