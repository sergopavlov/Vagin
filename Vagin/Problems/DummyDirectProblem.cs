using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vagin.interfaces;
using Vagin.Parameters;

namespace Vagin.Parameters
{
   internal class DummyOutput : ProblemOutputParameters
   {
      public DummyOutput(double value) { this.Value = value; }
      public double Value { get; private set; }
   }
   internal class DummyInput : ProblemInputParameters
   {
      public DummyInput(double point) { this.Point = point; }
      public double Point { get; set; }
   }
}
namespace Vagin.Problems
{
   internal abstract class DummyDirectProblem<Tinput, Toutput> : IDirectProblem<Tinput, Toutput> where Tinput : ProblemInputParameters where Toutput : ProblemOutputParameters
   {
      public abstract Toutput Calculate(Tinput parameters);

        public void Calculate(Tinput parametersm, Toutput output)
        {
            throw new NotImplementedException();
        }
    }

   internal class DummyDirectProblem : DummyDirectProblem<DummyInput, DummyOutput>
   {
      public override DummyOutput Calculate(DummyInput parameters)
      {
         return new DummyOutput(parameters.Point * parameters.Point + 4.0);
      }
   }
}
