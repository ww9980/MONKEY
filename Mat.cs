using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MathNet.Numerics;

namespace csMONKEY
{
    class Mat
    {
        public MathNet.Numerics.Complex32 Ri { get; set; }
        public double Wavelength { get; set; }
        public string MatName { get; set; }
        public string MatSymbol { get; set; }
    }

}
