using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vagin.Parameters;

namespace Vagin.interfaces
{
    internal interface IReverseProblem<Tinput,Toutput> where Tinput : ProblemOutputParameters where Toutput :  ProblemInputParameters
    {
        public Toutput Calculate(Tinput parameters, Toutput startValues);

    }
}
