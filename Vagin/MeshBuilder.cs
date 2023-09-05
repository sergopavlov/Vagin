using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vagin.interfaces;
using Vagin.Parameters;

namespace Vagin
{
   internal static class MeshBuilder
   {
      public static IMesh BuildMesh(MeshParameters parameters)
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
            R.Add(R[i - 1] + (R[i - 1]-R[i-2]) * parameters.RCoeff);
         }
         R.Add(parameters.RMax);
         mesh.SetR(R);

         var Z = new List<double>();
         var firstZ = Math.Abs(parameters.ZCoeff - 1) < 1e-12 ? (parameters.ZMax - parameters.ZMin) / parameters.ZsplitCount : (parameters.ZMax - parameters.ZMin) * (1 - parameters.ZCoeff) / (1 - Math.Pow(parameters.ZCoeff, (double)parameters.ZsplitCount));
         Z.Add(parameters.ZMax);
         Z.Add(parameters.ZMax - firstZ);
         for (int i = 2; i < parameters.ZsplitCount; i++)
         {
            Z.Add(Z[i - 1] + (Z[i - 1] - Z[i - 2]) * parameters.ZCoeff);
         }
         Z.Add(parameters.ZMin);
         mesh.SetZ(Z);

         var Elements = new List<Element>();
         var DirichleVertices = new HashSet<int>();
         for (int i = 0; i < Z.Count-1; i++)
         {
            for (int j = 0; j<R.Count-1; j++)
            {
               Elements.Add(new Element(new int[] { i * R.Count + j, i * R.Count + j + 1, (i + 1) * R.Count + j, (i + 1) * R.Count + j + 1 }));
               if (i == Z.Count - 2)
                  DirichleVertices.Add((i + 1) * R.Count + j);
               if (j == R.Count - 2)
                  DirichleVertices.Add(i * R.Count + j + 1);
               if (i == Z.Count - 2 && j == R.Count - 2)
                  DirichleVertices.Add((i + 1) * R.Count + j + 1);
            }
         }
         mesh.SetElements(Elements);
         mesh.SetDirichleCondition(DirichleVertices);

         throw new NotImplementedException();
      }
   }
}
