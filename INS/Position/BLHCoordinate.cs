

namespace INS
{
    internal class BLHCoordinate
    {

        public double B { get; set; }
        public double L { get; set; }
        public double H { get; set; }

        public BLHCoordinate(double B, double L, double H)
        {
            this.B = B;
            this.L = L;
            this.H = H;
        }

        public BLHCoordinate Clone()
        {
            return new(B, L, H);
        }

        public double this[int i]
        {
            get { if (i == 0) return Angle.Rad2Deg(B); if(i==1) return Angle.Rad2Deg(L); if(i==2) return H;throw new Exception(); }
        }

        public override string ToString()
        {
            return String.Format("({0,12:F4}° {1,12:F4}° {2,12:F4}m)", B * 180 / Math.PI, L * 180 / Math.PI, H);
        }

        public void SetValue(double b, double l, double h)
        {
            B = b; L = l; H = h;
        }
    }
}
