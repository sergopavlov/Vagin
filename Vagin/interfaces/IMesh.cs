using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vagin.interfaces
{
    internal interface IMesh
    {
        public IEnumerable<IElement> Elements { get; }
        public IReadOnlyList<double> R { get; }
        public IReadOnlyList<double> Z { get; }
        public IEnumerable<int> DirichleConditions { get; }
    }
}
