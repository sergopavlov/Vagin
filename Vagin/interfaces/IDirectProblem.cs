using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vagin.Parameters;
using Vagin.Problems;

namespace Vagin.interfaces
{
    internal interface IDirectProblem<Tinput, Toutput> where Tinput : ProblemInputParameters where Toutput : ProblemOutputParameters
    {
        public void Calculate(Tinput parametersm, Toutput output, DeltaType type);
    }
}
