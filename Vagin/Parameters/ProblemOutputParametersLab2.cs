using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vagin.Parameters
{
   internal class ProblemOutputParametersLab2 : ProblemOutputParameters
   {
      public List<Receiver> Receivers { get; set; }

      public object Clone()
      {
         var parameters = new ProblemOutputParametersLab1()
         {
            Receivers = new()
         };
         foreach (var item in Receivers)
         {
            parameters.Receivers.Add((Receiver)item.Clone());
         }
         return parameters;
      }
   }
}
