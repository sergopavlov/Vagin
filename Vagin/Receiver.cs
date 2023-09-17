using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vagin
{
    public class Receiver
    {
        public Receiver(double xN, double yN, double xM, double yM)
        {
            XN = xN;
            YN = yN;
            XM = xM;
            YM = yM;
        }

        public double XN { get; set; }
        public double YN { get; set; }
        public double XM { get; set; }
        public double YM { get; set; }
        public double V { get; set; }

    }
}
