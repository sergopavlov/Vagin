using Vagin;
using Vagin.Parameters;
using Vagin.Problems;

Console.WriteLine("Hello, World!");

MeshParameters tmp = new MeshParameters();
tmp.RMax = 10000;
tmp.RMin = 0;
tmp.RsplitCount = 50;
tmp.RCoeff = 2;

tmp.ZMax = 0;
tmp.ZMin = -10000;
tmp.ZsplitCount = 50;
tmp.ZCoeff = 2;
//var check = MeshBuilder.BuildMesh(tmp);
InverseProblem check = new();
DummyOutput output = (DummyOutput)check.Calculate(new DummyInput(0.1), new DummyOutput(0));
Console.WriteLine(output.Value);

