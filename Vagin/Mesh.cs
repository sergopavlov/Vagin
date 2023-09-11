using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vagin.interfaces;

namespace Vagin
{
   internal class Mesh : IMesh
   {
      public IEnumerable<IElement> Elements => elements.AsReadOnly();

      public IReadOnlyList<double> R => r.AsReadOnly();

      public IReadOnlyList<double> Z => z.AsReadOnly();

      public IEnumerable<int> DirichleConditions => dirichleConditions;

      private double[] r;
      private double[] z;
      private Element[] elements;
      private List<int> dirichleConditions;
      public void SetR(List<double> R, int Zcount)
      {
         var tmp = new List<double>();
         for (int i = 0; i<Zcount; i++)
         {
            tmp.AddRange(R);
         }
         r = tmp.ToArray();
      }
      public void SetZ(List<double> Z, int Rcount)
      {
         var tmp = new List<double>();
         for(int i = 0; i< Z.Count; i++)
         for (int j = 0; j < Rcount; j++)
         {
            tmp.Add(Z[i]);
         }
         z = tmp.ToArray();
      }
      public void SetElements(List<Element> Elements)
      {
         elements = Elements.ToArray();
      }

      public void SetDirichleCondition(HashSet<int> vertices)
      {
         dirichleConditions = vertices.ToList();
      }
   }
}
