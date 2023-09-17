using Vagin;
using Vagin.Parameters;
using Vagin.Problems;


MeshParameters tmp = new MeshParameters();
tmp.RMax = 10000;
tmp.RMin = 0;
tmp.RsplitCount = 200;
tmp.RCoeff = 1.04;

tmp.ZMax = 0;
tmp.ZMin = -10000;

tmp.ZsplitCount = 200;
tmp.ZCoeff = 1.04;
var check = MeshBuilder.BuildMesh(tmp);

FemProblemLab1 problem = new(check);
ProblemInputParametersLab1 parameters = new();
parameters.H1 = 10;
parameters.H2 = 20;
parameters.H3 = 30;
parameters.SourcePower = 1;
parameters.Sigma1 = 0.1;
parameters.Sigma2 = 0.2;
parameters.Sigma3 = 0.3;
parameters.Sigma4 = 0.4;

var output = new ProblemOutputParametersLab1()
{
    Receivers = new()
};
output.Receivers.Add(new Receiver(100, 0, 200, 0, 0));

problem.Calculate(parameters, output);
Lab1SourcepowerProblem problem1 = new(problem);
parameters.SourcePower = 100;
var res = problem1.Calculate(output, parameters);

Console.WriteLine("Hello, World!");
