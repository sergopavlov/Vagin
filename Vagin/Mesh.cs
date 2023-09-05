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
      public void SetR(List<double> R)
      {
         r = R.ToArray();
      }
      public void SetZ(List<double> Z)
      {
         z = Z.ToArray();
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
