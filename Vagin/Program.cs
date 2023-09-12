using Vagin;
using Vagin.Parameters;
using Vagin.Problems;

Console.WriteLine("Hello, World!");

MeshParameters tmp = new MeshParameters();
tmp.RMax = 10000;
tmp.RMin = 0;
tmp.RsplitCount = 200;
tmp.RCoeff = 1.07;

tmp.ZMax = 0;
tmp.ZMin = -10000;
tmp.ZsplitCount = 200;
tmp.ZCoeff = 1.07;
var check = MeshBuilder.BuildMesh(tmp);

FemProblemLab1 problem = new(check);
ProblemInputParametersLab1 parameters = new();
parameters.H1 = 10;
parameters.H2 = 20;
parameters.H3 = 30;
parameters.SourcePower = 1;
parameters.Sigma1 = 1;
parameters.Sigma2 = 1;
parameters.Sigma3 = 1;
parameters.Sigma4 = 1;

problem.Calculate(parameters);

