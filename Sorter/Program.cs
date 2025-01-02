using Sorter;
using Sorter.Common;
using Sorter.Phase2;

const string dir = "C:\\code\\LargeFileSorter\\TestFileGenerator\\bin\\Release\\net9.0\\";

const string path = dir + "100mb.txt";
//const string path = dir + "100mb.txt";


var runInfo = new Sorter.RunInfo(path);

//var intermediateChunkInfos = await Phase1Orchestrator.ExecutePhase1(runInfo);

var intermediateChunkInfos = await IntermediateChunkInfoDumper.ReadDump("1");
var info = intermediateChunkInfos[0];
using var reader = new Phase2SimpleChunkReader(info);

var rows = new List<Row>(GlobalSettings.ArrayPoolLengthLimit);

while (!reader.Completed)
{
    await reader.Reload();
    rows.Add(reader.Row);
}

var newInfo = info with { FilePath = TempFilePathFactory.CreateChunkFilePath("test", info.Id) };
await IntermidiateChunkStreamWriter.Write(newInfo, rows);

Console.WriteLine("end");


