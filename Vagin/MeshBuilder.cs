using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vagin.interfaces;
using Vagin.Parameters;
using Vagin.Problems;

namespace Vagin
{
   internal static class MeshBuilder
   {
      public static IMesh BuildMesh(MeshParameters parameters, DeltaType type)
      {
         Mesh mesh = new Mesh();
         parameters.ZsplitCount--;
         parameters.RsplitCount--;
         var R = new List<double>();
         var firstR = Math.Abs(parameters.RCoeff - 1) < 1e-12 ? (parameters.RMax - parameters.RMin) / parameters.RsplitCount : (parameters.RMax - parameters.RMin) * (1 - parameters.RCoeff) / (1 - Math.Pow(parameters.RCoeff, (double)parameters.RsplitCount));
         R.Add(parameters.RMin);
         R.Add(parameters.RMin + firstR);
         for (int i = 2; i < parameters.RsplitCount; i++)
         {
            R.Add(R[i - 1] + (R[i - 1] - R[i - 2]) * parameters.RCoeff);
         }
         R.Add(parameters.RMax);


         var Z = new List<double>();
         var firstZ = Math.Abs(parameters.ZCoeff - 1) < 1e-12 ? (parameters.ZMax - parameters.ZMin) / parameters.ZsplitCount : (parameters.ZMax - parameters.ZMin) * (1 - parameters.ZCoeff) / (1 - Math.Pow(parameters.ZCoeff, (double)parameters.ZsplitCount));
         Z.Add(parameters.ZMax);
         Z.Add(parameters.ZMax - firstZ);
         for (int i = 2; i < parameters.ZsplitCount; i++)
         {
            Z.Add(Z[i - 1] + (Z[i - 1] - Z[i - 2]) * parameters.ZCoeff);
         }
         Z.Add(parameters.ZMin);

         var tmp = new List<double>();
         for (int i = 0; i <= parameters.ZsplitCount; i++)
         {
            tmp.AddRange(R);
         }
         R = tmp;
         tmp = new List<double>();
         for (int i = 0; i <= parameters.ZsplitCount; i++)
            for (int j = 0; j <= parameters.RsplitCount; j++)
            {
               tmp.Add(Z[i]);
            }
         Z = tmp;
         var Elements = new List<Element>();
         if (type == DeltaType.noToK)
         {
            Z.RemoveAt(0);
            R.RemoveAt(0);
         }
         mesh.SetR(R);
         mesh.SetZ(Z);
         var DirichleVertices = new List<int>();
         for (int i = 0; i < parameters.ZsplitCount; i++)
         {
            for (int j = 0; j < parameters.RsplitCount; j++)
            {
               Elements.Add(new Element(new int[] { i * (parameters.RsplitCount + 1) + j, i * (parameters.RsplitCount + 1) + j + 1, (i + 1) * (parameters.RsplitCount + 1) + j, (i + 1) * (parameters.RsplitCount + 1) + j + 1 }));
               if (i == parameters.ZsplitCount - 1)
                  DirichleVertices.Add((i + 1) * (parameters.RsplitCount + 1) + j);
               if (j == parameters.RsplitCount - 1)
                  DirichleVertices.Add(i * (parameters.RsplitCount + 1) + j + 1);
               if (i == parameters.ZsplitCount - 1 && j == parameters.RsplitCount - 1)
                  DirichleVertices.Add((i + 1) * (parameters.RsplitCount + 1) + j + 1);
            }
         }
         if (type == DeltaType.noToK)
         {
            Elements.RemoveAt(0);
            foreach (var item in Elements)
            {
               for (int i = 0; i < 4; i++)
               {
                  item.LocalToGlobal[i]--;
               }
            }
            for (int i = 0; i < DirichleVertices.Count; i++)
            {
               DirichleVertices[i]--;
            }
         }

         mesh.SetElements(Elements);
         mesh.SetDirichleCondition(DirichleVertices.Distinct().ToList());
         if (type == DeltaType.noToK)
         {
            mesh.SetNeumanCondition(new List<(int, int, double)>
            {
               (0, 100, 1 / (-2 * Math.PI * Z[100] * R[0] + Math.PI * R[0] * R[0])),
               (99, 100, 1 / (-2 * Math.PI * Z[100] * R[0] + Math.PI * R[0] * R[0]))
            });
         }

         return mesh;
      }
   }
}
