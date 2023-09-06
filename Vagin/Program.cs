
using Vagin;
using Vagin.Parameters;

Console.WriteLine("Hello, World!");

MeshParameters tmp = new MeshParameters();
tmp.RMax = 10;
tmp.RMin = 0;
tmp.RsplitCount = 5;
tmp.RCoeff = 2;

tmp.ZMax = 0;
tmp.ZMin = -10;
tmp.ZsplitCount = 3;
tmp.ZCoeff = 2;
var check = MeshBuilder.BuildMesh(tmp);