using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vagin.Parameters
{
    internal class MeshParameters
    {
        public double RMin { get; set; }
        public double RMax { get; set; }
        public int RsplitCount { get; set; }
        public double RCoeff { get; set; }
        public double ZMin { get; set; }
        public double ZMax { get; set; }
        public int ZsplitCount { get; set; }
        public double ZCoeff { get; set; }
    }
}
