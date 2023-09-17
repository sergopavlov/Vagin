using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vagin.Parameters;

namespace Vagin.Problems
{
    internal class Lab1SourcepowerProblem : InverseProblemBase<ProblemOutputParametersLab1, ProblemInputParametersLab1>
    {
        FemProblemLab1 directProblem;
        int Maxiter = 1000;
        double A, F, deltaPower;
        public Lab1SourcepowerProblem(FemProblemLab1 directProblem)
        {
            this.directProblem = directProblem;
        }

        public override ProblemInputParametersLab1 Calculate(ProblemOutputParametersLab1 parameters, ProblemInputParametersLab1 startValues)
        {
            var parameter = startValues;
             deltaPower = 1; // по идее любое число отличное от 0, логично что в искомой задаче H3 будет не 0, но на тестовых задачах лучше за эти следить
                                // проинициализировать прямую проблему
                                // проинициализировать Experimental

            for (var Iterations = 0; Iterations < Maxiter || Math.Abs(deltaPower) > 1e-14; Iterations++)
            {
                if (Assembly(parameter, parameters) < 0)
                    break;
                Gauss();
                parameter.SourcePower += deltaPower;
            }
            return startValues;// засунуть H3 в ProblemOutputParameters и вернуть
            throw new NotImplementedException();
        }
        private int Assembly(ProblemInputParametersLab1 parameters, ProblemOutputParametersLab1 parametersout)
        {
            int n = parametersout.Receivers.Count;
            var calcnodiff = (ProblemOutputParametersLab1)parametersout.Clone();
            directProblem.Calculate(parameters, calcnodiff);
            double[] V = new double[n];
            for (int i = 0; i < n; i++)
            {
                V[i] = calcnodiff.Receivers[i].V;
            }
            double dH = 0.05 * parameters.SourcePower; // 0.05 взято из методы с12 п3
            //var r = CalcF(parameters, parametersout);
            parameters.SourcePower += dH;
            var calcdiff = (ProblemOutputParametersLab1)parametersout.Clone();
            directProblem.Calculate(parameters, calcdiff);
            double[] dV = new double[parametersout.Receivers.Count];
            for (int i = 0; i < dV.Length; i++)
            {
                dV[i] = (calcdiff.Receivers[i].V - V[i]) / dH;
            }
            A = 0;
            F = 0;
            parameters.SourcePower -= dH;
            for (int i = 0; i < parametersout.Receivers.Count; i++)
            {
                A += dV[i] * dV[i];
                F -= dV[i] * (calcnodiff.Receivers[i].V - parametersout.Receivers[i].V);
            }
            double penalty = 0;
            for (int i = 0; i < n; i++)
            {
                penalty += (calcnodiff.Receivers[i].V - parametersout.Receivers[i].V) * (calcnodiff.Receivers[i].V - parametersout.Receivers[i].V);
            }
            if (Math.Abs(A) < 1e-14||penalty<1e-14)
                return -1;
            return 0;
        }

        private void Gauss()
        {
            deltaPower = F / A;
        }
    }
}
