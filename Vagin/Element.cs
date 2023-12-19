using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vagin.interfaces;

namespace Vagin
{
   internal class Element : IElement
   {
      public int[] LocalToGlobal { get; }

      public Element(int[] vertices)
      {
         LocalToGlobal = vertices;
      }

      
   }
}
