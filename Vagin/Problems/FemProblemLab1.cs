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
        public FemProblemLab1(IMesh mesh) : base(mesh)
        {
        }

        public override ProblemOutputParametersLab1 Calculate(ProblemInputParametersLab1 parameters)
        {
            SolveFem(parameters);
            return null;
        }

        protected override double CalcAverageSigma(IElement element, ProblemInputParametersLab1 parameters)
        {
            var elembounds = this.GetElemBoundaries(element);
            var h1sigma = (elembounds.zmax - parameters.H1) / (elembounds.zmax - elembounds.zmin);
            h1sigma = h1sigma >= 0 && h1sigma <= 1 ? h1sigma : 0;
            var h2sigma = (elembounds.zmax - (parameters.H2 + parameters.H1)) / (elembounds.zmax - elembounds.zmin);
            h2sigma = h2sigma >= 0 && h2sigma <= 1 ? h2sigma : 0;
            var h3sigma = (elembounds.zmax - (parameters.H1 + parameters.H2 + parameters.H3)) / (elembounds.zmax - elembounds.zmin);
            h3sigma = h3sigma >= 0 && h3sigma <= 1 ? h3sigma : 0;
            var h4sigma = h1sigma + h2sigma + h3sigma < 1 ? 1 - (h1sigma + h2sigma + h3sigma) : 0;
            h1sigma *= parameters.Sigma1;
            h2sigma *= parameters.Sigma2;
            h3sigma *= parameters.Sigma3;
            h4sigma *= parameters.Sigma4;
            return h1sigma + h2sigma + h3sigma + h4sigma;
        }

        protected override double[] GetLocalRightPart(IElement element)
        {
            if (!IsPointInsideElement(element, 0, 0))
            {
                return new double[4];
            }
            else
            {
                return new double[] { 1.0 / (2 * Math.PI), 0, 0, 0 };
            }
        }
    }
}
