using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vagin.Parameters;

namespace Vagin.interfaces
{
    internal interface IReverseProblem
    {
        public ProblemOutputParameters Calculate(ProblemInputParameters parameters, ProblemOutputParameters startValues);

    }
}
