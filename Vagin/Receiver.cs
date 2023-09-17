using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vagin
{
    public class Receiver : ICloneable
    {
        public Receiver(double xN, double yN, double xM, double yM, double v)
        {
            XN = xN;
            YN = yN;
            XM = xM;
            YM = yM;
            V = v;
        }

        public double XN { get; set; }
        public double YN { get; set; }
        public double XM { get; set; }
        public double YM { get; set; }
        public double V { get; set; }

        public object Clone()
        {
            return new Receiver(XN, YN, XM, YM,V);
        }
    }
}
