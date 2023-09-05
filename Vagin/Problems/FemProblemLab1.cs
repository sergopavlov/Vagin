using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vagin.interfaces;
using Vagin.Parameters;

namespace Vagin.Problems
{
    internal class FemProblemLab1 : FemProblemBase<ProblemInputParametersLab1, ProblemOutputParametersLab1>
    {
        public override ProblemOutputParametersLab1 Calculate(ProblemInputParametersLab1 parameters)
        {
            throw new NotImplementedException();
        }

        protected override double CalcAverageSigma(IElement element, ProblemInputParametersLab1 parameters)
        {
            
        }

        protected override double[] GetLocalRightPart(IElement element)
        {
            if (!IsPointInsideElement(element, 0, 0))
            {
                return new double[4];
            }
            else
            {

            }
        }
    }
}
