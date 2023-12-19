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
      public IEnumerable<IElement> Elements => elements;

      public IReadOnlyList<double> R => r.AsReadOnly();

      public IReadOnlyList<double> Z => z.AsReadOnly();

      public IEnumerable<int> DirichleConditions => dirichleConditions;
      public IEnumerable<(int, int, double)> NeumanConditions => neumanConditions;

      private double[] r;
      private double[] z;
      private Element[] elements;
      private List<int> dirichleConditions;
      private List<(int, int, double)> neumanConditions;
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

      public void SetDirichleCondition(List<int> vertices)
      {
         dirichleConditions = vertices.ToList();
      }

      public void SetNeumanCondition(List<(int, int, double)> edges)
      {
         neumanConditions = edges;
      }
   }
}
