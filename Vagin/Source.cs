using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vagin
{
   internal class Source
   {
      public Source(double xA, double yA, double xB, double yB, double i)
      {
         XA = xA;
         YA = yA;
         XB = xB;
         YB = yB;
         I = i;
      }

      public double XA { get; set; }
      public double YA { get; set; }
      public double XB { get; set; }
      public double YB { get; set; }
      public double I { get; set; }
   }
}
