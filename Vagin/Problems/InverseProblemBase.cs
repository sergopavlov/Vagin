using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vagin.interfaces;
using Vagin.Parameters;

namespace Vagin.Problems
{
   internal abstract class InverseProblemBase<Tinput, Toutput> : IReverseProblem<Tinput, Toutput> where Tinput :  ProblemOutputParameters where Toutput : ProblemInputParameters
    {
      private double Experimental = 13.0;
      private int Iterations = 0; // Счётчик итераций (для отчёта)
      private int Maxiter = 100;
      private double A;
      private double F;
      private double Result; // delta H
      private double H3; // начальное приближение H3
      private DummyDirectProblem<DummyInput, DummyOutput> dummyDirectProblem = new DummyDirectProblem();

      public ProblemOutputParameters Calculate(ProblemInputParameters parameters, ProblemOutputParameters startValues)
      {
         this.H3 = ((DummyInput)parameters).Point;// проинициализировать начальное приближение H3
         Result = H3; // по идее любое число отличное от 0, логично что в искомой задаче H3 будет не 0, но на тестовых задачах лучше за эти следить
         // проинициализировать прямую проблему
         // проинициализировать Experimental
         for (Iterations = 0; Iterations < Maxiter || Math.Abs(Result) > 1e-14; Iterations++)
         {
            if (Assembly() < 0)
               break;
            Gauss();
            H3 += Result;
         }
         return new DummyOutput(H3);// засунуть H3 в ProblemOutputParameters и вернуть
      }
        public abstract Toutput Calculate(Tinput parameters, Toutput startValues);

        private int Assembly()
      {
         double dH = 0.05 * H3; // 0.05 взято из методы с12 п3
         double tmp = dummyDirectProblem.Calculate(new DummyInput(H3)).Value;
         double dVdH = (dummyDirectProblem.Calculate(new DummyInput(H3 + dH)).Value - tmp) / dH;
         A = dVdH * dVdH;
         F = dVdH * (Experimental - tmp);
         if (Math.Abs(A) < 1e-14)
            return -1;
         return 0;
      }

      private void Gauss()
      {
         Result = F / A;
      }

   }
}
