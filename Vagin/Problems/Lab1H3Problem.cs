using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vagin.Parameters;

namespace Vagin.Problems
{
   internal class Lab1H3Problem : InverseProblemBase<ProblemOutputParametersLab1, ProblemInputParametersLab1>
   {

      FemProblemLab1 directProblem;
      int Maxiter = 1000;
      double A, F, deltaPower, penalty;
      public Lab1H3Problem(FemProblemLab1 directProblem)
      {
         this.directProblem = directProblem;
      }

      public override ProblemInputParametersLab1 Calculate(ProblemOutputParametersLab1 parameters, ProblemInputParametersLab1 startValues)
      {
         var parameter = startValues;
         deltaPower = 1; // по идее любое число отличное от 0, логично что в искомой задаче H3 будет не 0, но на тестовых задачах лучше за эти следить
                         // проинициализировать прямую проблему
                         // проинициализировать Experimental
         penalty = 1;
         Console.WriteLine($"iter = {0} h3 = {parameter.H3} penalty = {-1}");
         for (var Iterations = 1; Iterations < Maxiter && penalty > 1e-15 && Math.Abs(deltaPower)>1e-4; Iterations++)
         {
            Assembly(parameter, parameters);
            Gauss();
            var old = parameter.H3;
            var lastpenalty = 1;
            bool flag = true;
            /*while (flag)
            {
               parameter.H3 += deltaPower;
               var outparams = (ProblemOutputParametersLab1)parameters.Clone();
               directProblem.Calculate(parameter, outparams);
               var p  = CalcPenalty(outparams, parameters);
               if(p<lastpenalty)
               {
                  parameter.H3 = old + deltaPower * 2;
                  deltaPower *= 2;
                  Console.WriteLine(parameter.H3);
               }
               else
               {
                  flag = false;
               }
               
            }*/
            parameter.H3 += deltaPower;
            Console.WriteLine($"iter = {Iterations} h3 = {parameter.H3} penalty = {penalty}");
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
         double dH = 0.05 * parameters.H3; // 0.05 взято из методы с12 п3
                                           //var r = CalcF(parameters, parametersout);
         parameters.H3 += dH;
         var calcdiff = (ProblemOutputParametersLab1)parametersout.Clone();
         directProblem.Calculate(parameters, calcdiff);
         double[] dV = new double[parametersout.Receivers.Count];
         for (int i = 0; i < dV.Length; i++)
         {
            dV[i] = (calcdiff.Receivers[i].V - V[i]) / dH;
         }
         A = 0;
         F = 0;
         parameters.H3 -= dH;
         for (int i = 0; i < parametersout.Receivers.Count; i++)
         {
            A += dV[i] * dV[i] / parametersout.Receivers[i].V / parametersout.Receivers[i].V;
            F -= dV[i] * (calcnodiff.Receivers[i].V - parametersout.Receivers[i].V) / parametersout.Receivers[i].V / parametersout.Receivers[i].V;
         }
         penalty = 0;
         for (int i = 0; i < n; i++)
         {
            penalty += (calcnodiff.Receivers[i].V - parametersout.Receivers[i].V) * (calcnodiff.Receivers[i].V - parametersout.Receivers[i].V);
         }
         if (Math.Abs(A) < 1e-14 || penalty < 1e-15)
            return -1;
         return 0;
      }

      private double CalcPenalty(ProblemOutputParametersLab1 calc, ProblemOutputParametersLab1 truee)
      {
         int n = calc.Receivers.Count;
         double penalty = 0;
         for (int i = 0; i < n; i++)
         {
            penalty += (calc.Receivers[i].V - truee.Receivers[i].V) * (calc.Receivers[i].V - truee.Receivers[i].V);
         }
         return penalty;
      }
      private void Gauss()
      {
         deltaPower = F / A;
      }

   }
}
