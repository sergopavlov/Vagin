using Vagin;
using Vagin.Parameters;
using Vagin.Problems;


MeshParameters tmp = new MeshParameters();
tmp.RMax = 10000;
tmp.RMin = 0;
tmp.RsplitCount = 100;
tmp.RCoeff = 1.04;

tmp.ZMax = 0;
tmp.ZMin = -10000;

tmp.ZsplitCount = 100;
tmp.ZCoeff = 1.04;
var check = MeshBuilder.BuildMesh(tmp);

FemProblemLab1 problem = new(check);
ProblemInputParametersLab1 parameters = new();
parameters.H1 = 5;
parameters.H2 = 10;
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
output.Receivers.Add(new Receiver(300, 0, 400, 0, 0));


problem.Calculate(parameters, output);
//Lab1SourcepowerProblem problem2 = new(problem);
//parameters.SourcePower = 100;

output.Receivers[0].V *= 1.01;
Lab1H3Problem problem2 = new(problem);
parameters.H3 = 50;
var res = problem2.Calculate(output, parameters);

Console.WriteLine("Hello, World!");
