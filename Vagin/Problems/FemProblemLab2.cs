using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vagin.interfaces;
using Vagin.Parameters;

namespace Vagin.Problems
{
   internal class FemProblemLab2 : FemProblemBase<ProblemInputParametersLab2, ProblemOutputParametersLab2>
   {

      public FemProblemLab2(IMesh mesh) : base(mesh)
      {

      }

      public override void Calculate(ProblemInputParametersLab2 parameters, ProblemOutputParametersLab2 output, DeltaType type)
      {
         SolveFem(parameters, type);
         foreach (var item in output.Receivers)
         {
            item.V = GetSolutionAtpoint(Math.Sqrt(item.XM * item.XM + item.YM * item.YM), 0) - GetSolutionAtpoint(Math.Sqrt(item.XN * item.XN + item.YN * item.YN), 0);
         }
      }

      protected override double CalcAverageSigma(IElement element, ProblemInputParametersLab2 parameters)
      {
         var elembounds = this.GetElemBoundaries(element);
         if (elembounds.zmin >= -parameters.H1)
            return parameters.Sigma1;
         if (elembounds.zmax <= -parameters.H1 && elembounds.zmin >= -(parameters.H1 + parameters.H2))
            return parameters.Sigma2;
         if (elembounds.zmax <= -(parameters.H1 + parameters.H2) && elembounds.zmin >= -(parameters.H1 + parameters.H2 + parameters.H3))
            return parameters.Sigma3;
         if (elembounds.zmax <= -(parameters.H1 + parameters.H2 + parameters.H3))
            return parameters.Sigma4;
         var h1sigma = (elembounds.zmax + parameters.H1) / (elembounds.zmax - elembounds.zmin);
         h1sigma = h1sigma >= 0 && h1sigma <= 1 ? h1sigma : 0;
         if (h1sigma > 0)
            return h1sigma * parameters.Sigma1 + (1 - h1sigma) * parameters.Sigma2;
         var h2sigma = (elembounds.zmax + (parameters.H2 + parameters.H1)) / (elembounds.zmax - elembounds.zmin);
         h2sigma = h2sigma >= 0 && h2sigma <= 1 ? h2sigma : 0;
         if (h2sigma > 0)
            return h2sigma * parameters.Sigma2 + (1 - h2sigma) * parameters.Sigma3;
         var h3sigma = (elembounds.zmax + (parameters.H1 + parameters.H2 + parameters.H3)) / (elembounds.zmax - elembounds.zmin);
         h3sigma = h3sigma >= 0 && h3sigma <= 1 ? h3sigma : 0;
         if (h3sigma > 0)
            return h3sigma * parameters.Sigma3 + (1 - h3sigma) * parameters.Sigma4;
         var h4sigma = h1sigma + h2sigma + h3sigma < 1 ? 1 - (h1sigma + h2sigma + h3sigma) : 0;
         h1sigma *= parameters.Sigma1;
         h2sigma *= parameters.Sigma2;
         h3sigma *= parameters.Sigma3;
         h4sigma *= parameters.Sigma4;
         return h1sigma + h2sigma + h3sigma + h4sigma;
      }

      protected override double[] GetLocalRightPart(IElement element, ProblemInputParametersLab2 parameters, DeltaType type)
      {
         if (!IsPointInsideElement(element, 0, 0))
         {
            return new double[4];
         }
         else
            return new double[] { parameters.SourcePower / (2 * Math.PI), 0, 0, 0 };
      }
   }
}
