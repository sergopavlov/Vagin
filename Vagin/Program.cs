using Vagin;
using Vagin.Parameters;

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
var check = MeshBuilder.BuildMesh(tmp);

