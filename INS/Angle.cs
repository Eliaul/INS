using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INS
{
    internal struct Angle
    {
        public static double Deg2Rad(double deg)
        {
            return deg * Math.PI / 180;
        }

        public static double Rad2Deg(double rad)
        {
            return rad * 180 / Math.PI; 
        }
    }
}
