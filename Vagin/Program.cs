using System.Globalization;
using System.Text.Json;
using System.Transactions;
using Vagin;
using Vagin.Parameters;
using Vagin.Problems;

CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

MeshParameters tmp = new MeshParameters();
tmp.RMax = 10000;
tmp.RMin = 0;
tmp.RsplitCount = 100;
tmp.RCoeff = 1.04;

tmp.ZMax = 0;
tmp.ZMin = -10000;

tmp.ZsplitCount = 100;
tmp.ZCoeff = 1.04;
var mesh = MeshBuilder.BuildMesh(tmp, DeltaType.delta);

/*FemProblemLab1 problem = new(mesh);
ProblemInputParametersLab1 parameters = new();
parameters.H1 = 205;
parameters.H2 = 200;
parameters.H3 = 300;
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

problem.Calculate(parameters, output, DeltaType.noToK);

List<Source> sources = new()
{
   new Source(0,-500,100,-500,1),
   new Source(0,0,100,0,1),
   new Source(0,500,100,500,1)
};

List<Receiver> receivers = new()
{
   new(200,0,300,0,0),
   new(500,0,600,0,0),
   new(1000,0,1100,0,0)
};
CalcV();

for (int i = 0; i < receivers.Count; i++)
{
   Console.WriteLine($"{i} {receivers[i].V}");

}


void CalcV()
{
   foreach (var recv in receivers)
   {
      foreach (var source in sources)
      {
         double Vm = problem.GetSolutionAtpoint(Math.Sqrt((source.XA - recv.XM) * (source.XA - recv.XM) + (source.YA - recv.YM) * (source.YA - recv.YM)), 0);
         Vm -= problem.GetSolutionAtpoint(Math.Sqrt((source.XB - recv.XM) * (source.XB - recv.XM) + (source.YB - recv.YM) * (source.YB - recv.YM)), 0);
         double Vn = problem.GetSolutionAtpoint(Math.Sqrt((source.XA - recv.XN) * (source.XA - recv.XN) + (source.YA - recv.YN) * (source.YA - recv.YN)), 0);
         Vn -= problem.GetSolutionAtpoint(Math.Sqrt((source.XB - recv.XN) * (source.XB - recv.XN) + (source.YB - recv.YN) * (source.YB - recv.YN)), 0);
         recv.V += (Vm - Vn) * source.I;
      }
   }
}*/




List<Source> sources = new()
{
   new Source(10,100,10,110,1),
   new Source(30,100,30,110,1),
   new Source(50,100,50,110,1),
   new Source(70,100,70,110,1),
   new Source(70,100,70,110,1),
   new Source(70,100,70,110,1),
   new Source(70,100,70,110,1),
   new Source(70,100,70,110,1),
   new Source(70,100,70,110,1),
   new Source(70,100,70,110,1),
   new Source(70,100,70,110,1),
};

List<Receiver> receivers = new()
{
   new(10,40,40,40,0),
   new(30,30,60,30,0),
   new(50,20,80,20,0)
};


FemProblemLab2 problem = new(mesh);

void GenerateV()
{
    //foreach (var source in sources)
    var source = sources[1];
    {
        for (int i = 0; i < receivers.Count; i++)
        {
            Receiver? recv = receivers[i];
            recv.V = 0;
            double Vm = problem.GetSolutionAtpoint(Math.Sqrt((source.XA - recv.XM) * (source.XA - recv.XM) + (source.YA - recv.YM) * (source.YA - recv.YM)), 0);
            Vm -= problem.GetSolutionAtpoint(Math.Sqrt((source.XB - recv.XM) * (source.XB - recv.XM) + (source.YB - recv.YM) * (source.YB - recv.YM)), 0);
            double Vn = problem.GetSolutionAtpoint(Math.Sqrt((source.XA - recv.XN) * (source.XA - recv.XN) + (source.YA - recv.YN) * (source.YA - recv.YN)), 0);
            Vn -= problem.GetSolutionAtpoint(Math.Sqrt((source.XB - recv.XN) * (source.XB - recv.XN) + (source.YB - recv.YN) * (source.YB - recv.YN)), 0);
            recv.V += (Vm - Vn) * source.I;
        }
        receivers[1].V *= 1.06;
    }
}

(int index, double residual) CalcV(List<Receiver> Synthetic)
{
    var min = 99999999.9;
    var index = -1;
    var residual = 0.0;
    var receiversForISource = new List<List<Receiver>>();
    for (int i = 0; i < sources.Count; i++)
    {
        receiversForISource.Add(new List<Receiver>());
        for (int j = 0; j < receivers.Count(); j++)
        {
            receiversForISource[i].Add((Receiver)receivers[j].Clone());
            receiversForISource[i][j].V = 0;
        }
        var source = sources[i];
        foreach (var recv in receiversForISource[i])
        {
            double Vm = problem.GetSolutionAtpoint(Math.Sqrt((source.XA - recv.XM) * (source.XA - recv.XM) + (source.YA - recv.YM) * (source.YA - recv.YM)), 0);
            Vm -= problem.GetSolutionAtpoint(Math.Sqrt((source.XB - recv.XM) * (source.XB - recv.XM) + (source.YB - recv.YM) * (source.YB - recv.YM)), 0);
            double Vn = problem.GetSolutionAtpoint(Math.Sqrt((source.XA - recv.XN) * (source.XA - recv.XN) + (source.YA - recv.YN) * (source.YA - recv.YN)), 0);
            Vn -= problem.GetSolutionAtpoint(Math.Sqrt((source.XB - recv.XN) * (source.XB - recv.XN) + (source.YB - recv.YN) * (source.YB - recv.YN)), 0);
            recv.V += (Vm - Vn) * source.I;
        }
    }
    for (int i = 0; i < sources.Count; i++)
    {
        residual = 0.0;
        for (int j = 0; j < receivers.Count; j++)
        {
            residual += Math.Abs(receiversForISource[i][j].V - Synthetic[j].V);
        }
        if (residual < min)
        {
            min = residual;
            index = i;
        }
    }
    return (index, min);
}

ProblemInputParametersLab2 input = new();
input.Sources = sources;

input.H1 = 205;
input.H2 = 200;
input.H3 = 300;
input.SourcePower = 1;
input.Sigma1 = 0.1;
input.Sigma2 = 0.2;
input.Sigma3 = 0.3;
input.Sigma4 = 0.4;

var output = new ProblemOutputParametersLab2();
output.Receivers = receivers;

problem.Calculate(input, output, DeltaType.delta);
GenerateV();
int ind;
double residual;
(ind, residual) = CalcV(receivers);
Console.WriteLine(ind);
Console.WriteLine(residual);


