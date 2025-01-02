using Sorter.Common;
using Sorter.Phase1;

const string dir = "C:\\code\\LargeFileSorter\\TestFileGenerator\\bin\\Release\\net9.0\\";

const string path = dir + "100mb.txt";
//const string path = dir + "100mb.txt";


var runInfo = new Sorter.RunInfo(path);

//var intermediateChunkInfos = await Phase1Orchestrator.ExecutePhase1(runInfo);

var intermediateChunkInfos = await IntermediateChunkInfoDumper.ReadDump("1");


Console.WriteLine("end");


