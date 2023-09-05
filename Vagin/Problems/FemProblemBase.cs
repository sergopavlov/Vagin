using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vagin.interfaces;

namespace Vagin.Problems
{
    internal abstract class FemProblemBase
    {
        IMesh mesh;

        protected bool IsPointInsideElement(IElement element, double r, double z)
        {

        }
    }
}
