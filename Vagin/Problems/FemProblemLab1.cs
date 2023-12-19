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

      public override void Calculate(ProblemInputParametersLab1 parameters, ProblemOutputParametersLab1 output, DeltaType type)
      {
         SolveFem(parameters, type);
         foreach (var item in output.Receivers)
         {
            item.V = GetSolutionAtpoint(Math.Sqrt(item.XM * item.XM + item.YM * item.YM), 0) - GetSolutionAtpoint(Math.Sqrt(item.XN * item.XN + item.YN * item.YN), 0);
         }

      }

      protected override double CalcAverageSigma(IElement element, ProblemInputParametersLab1 parameters)
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


      protected override double[] GetLocalRightPart(IElement element, ProblemInputParametersLab1 parameters, DeltaType type)
      {

         switch (type)
         {
            case DeltaType.delta:
               if (!IsPointInsideElement(element, 0, 0))
               {
                  return new double[4];
               }
               else
                  return new double[] { parameters.SourcePower / (2 * Math.PI), 0, 0, 0 };
               break;
            case DeltaType.pazmaznya:
               if (!IsPointInsideElement(element, 0, 0))
               {
                  return new double[4];
               }
               else
               {
                  double[] rs = new double[2]
                  {
                     mesh.R[element.LocalToGlobal[0]],
                     mesh.R[element.LocalToGlobal[1]]
                  };
                  var dims = GetElemBoundaries(element);
                  double hr = dims.rmax - dims.rmin;
                  double hz = dims.zmax - dims.zmin;
                  var res = new double[4];
                  double source = parameters.SourcePower / (Math.PI * hr * hr * hz);
                  for (int i = 0; i < 4; i++)
                  {
                     for (int j = 0; j < 4; j++)
                     {
                        for (int k = 0; k < 2; k++)
                        {
                           res[i] += rs[k] * hr * hz * Matrices.MZ[i / 2][j / 2] * Matrices.MR[k][i % 2][j % 2] * source;
                        }
                     }
                  }
                  return res;
               }
               break;
            case DeltaType.noToK:

               var rez = new double[4];
               if (element.LocalToGlobal.Contains(0))
               {
                  var cond = mesh.NeumanConditions.Where(t => t.Item1 == 0).First();
                  var cur = GetElemBoundaries(element);
                  rez[0] += 0.5 * cond.Item3 * (cur.rmin) * (cur.zmax - cur.zmin);
                  rez[2] += 0.5 * cond.Item3 * (cur.rmin) * (cur.zmax - cur.zmin);
               }
               if (element.LocalToGlobal.Contains(99))
               {
                  var cond = mesh.NeumanConditions.Where(t => t.Item1 == 99).First();
                  var cur = GetElemBoundaries(element);
                  rez[0] += 1.0 / 6 * cond.Item3 * (cur.rmax - cur.rmin) * (cur.rmax - cur.rmin);
                  rez[1] += 1.0 / 3 * cond.Item3 * (cur.rmax - cur.rmin) * (cur.rmax - cur.rmin);
               }
               //   throw new Exception("idi");
               return rez;
               //throw new Exception("idi");
               break;
            default:
               throw new Exception("idi");
               break;
         }

      }
   }
   public enum DeltaType
   {
      delta,
      pazmaznya,
      noToK,
   }
}
