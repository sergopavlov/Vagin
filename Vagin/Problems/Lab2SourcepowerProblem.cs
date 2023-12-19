using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vagin.Parameters;

namespace Vagin.Problems
{
   internal class Lab2SourcepowerProblem : InverseProblemBase<ProblemOutputParametersLab2, ProblemInputParametersLab2>
   {
      FemProblemLab2 directProblem;
      int Maxiter = 1000;
      double[,] A;
      double[] F, deltaPower;
      double penalty;
      public Lab2SourcepowerProblem(FemProblemLab2 directProblem)
      {
         this.directProblem = directProblem;
      }
      public override ProblemInputParametersLab2 Calculate(ProblemOutputParametersLab2 parameters, ProblemInputParametersLab2 startValues)
      {
         var parameter = startValues;
         deltaPower[0] = 1; // по идее любое число отличное от 0, логично что в искомой задаче H3 будет не 0, но на тестовых задачах лучше за эти следить
                            // проинициализировать прямую проблему
                            // проинициализировать Experimental
         penalty = 1;
         Console.WriteLine($"iter = {0} power = {parameter.SourcePower} penalty = {-1}");
         for (var Iterations = 1; Iterations < Maxiter && penalty > 1e-15; Iterations++)
         {
            Assembly(parameter, parameters);
            Gauss();
            for (int i = 0; i < deltaPower.Count(); i++)
               parameter.Sources[i].I += deltaPower[i];
            Console.WriteLine($"iter = {Iterations} power = {parameter.SourcePower} penalty = {penalty}");
         }
         return startValues;// засунуть H3 в ProblemOutputParameters и вернуть
      }

      private void Assembly(ProblemInputParametersLab2 parameters, ProblemOutputParametersLab2 parametersout)
      {
         int n = parameters.Sources.Count;
         var calcnodiff = (ProblemOutputParametersLab2)parametersout.Clone();
         directProblem.Calculate(parameters, calcnodiff, DeltaType.delta);
         double[] V = new double[n];
         for (int i = 0; i < n; i++)
         {
            V[i] = calcnodiff.Receivers[i].V;
         }
         double dH = 0.05 * parameters.SourcePower; // 0.05 взято из методы с12 п3
                                                    //var r = CalcF(parameters, parametersout);
         parameters.SourcePower += dH;
         var calcdiff = (ProblemOutputParametersLab2)parametersout.Clone();
         directProblem.Calculate(parameters, calcdiff, DeltaType.delta);
         double[] dV = new double[parametersout.Receivers.Count];
         for (int i = 0; i < dV.Length; i++)
         {
            dV[i] = (calcdiff.Receivers[i].V - V[i]) / dH;
         }
         A = new double[n, n];
         F = new double[n];
         parameters.SourcePower -= dH;
         for (int i = 0; i < parametersout.Receivers.Count; i++)
         {
            for (int j = 0; j < n; j++)
            {
               for (int k = 0; k < n; k++)
                  A[j, k] += dV[j] * dV[k];
               F[j] -= dV[j] * (calcnodiff.Receivers[i].V - parametersout.Receivers[i].V);
            }

         }
         penalty = 0;
         for (int i = 0; i < n; i++)
         {
            penalty += (calcnodiff.Receivers[i].V - parametersout.Receivers[i].V) * (calcnodiff.Receivers[i].V - parametersout.Receivers[i].V);
         }
         Console.WriteLine($"penalty {penalty}");
      }
      private void Gauss()
      {
         double m=0;
         int n = F.Count();
         for (int i = 0; i < n; i++)
         {
            // Ищем максимальный элемент в столбце
            int max = i;
            for (int j = i + 1; j < n; j++)
               if (Math.Abs(A[i, j]) > Math.Abs(A[i, max]))
                  max = j;

            // Меняем текущую строку со строкой, в которой
            // находится максимальный элемент
            double tmp;
            for (int j = i; j < n; j++)
            {
               tmp = A[max, j];
               A[max, j] = A[i, j];
               A[i, j] = tmp;
            }
            tmp = F[i];
            F[i]=F[max];
            var bmax = tmp;

            // Вычитаем из всех строк, расположенных ниже
            // текущей, строку с максимальным элементом,
            // умноженную на вычисленный коэффициент.
            // То же самое делаем для вектора
            for (int j = i + 1; j < n; j++)
            {
               m = A[j,i] / A[i,i];
               F[j] -= m * F[i];
               for (int k = i; k < n; k++)
                  A[j,k] -= m * A[i,k];
            }
            m = A[i,i];
            for (int j = i; j < n; j++)
               A[i,j] /= m;
            F[i] /= m;
         }

         // Обратный ход
         for (int i = n - 1; i >= 0; i--)
         {
            double prod = F[i];
            for (int j = n - 1; j >= i; j--)
               prod -= A[i,j] * deltaPower[j];
            prod /= A[i, i];
            deltaPower[i] = prod;
         }
      }

   }
}
