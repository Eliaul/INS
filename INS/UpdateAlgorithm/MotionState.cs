

namespace INS
{
    internal class MotionState
    {
        
        public Quaternion Attitude { get; set; } = default!;
        public BLHCoordinate BLH { get; set; } = default!;
        /// <summary>
        /// NED
        /// </summary>
        public Vector3d Velocity { get; set; } = default!;
        public Vector3d Gravity { get; set; } = default!;
        public Vector3d Omega_ie { get; set; } = default!;
        public Vector3d Omega_en { get; set; } = default!;
        public double R_N { get; set; } = default!;
        public double R_M { get; set; } = default!;
        public double GPSSec { get; set; } = default!;

        public MotionState(Quaternion attitude, BLHCoordinate bLHCoordinate, Vector3d velocity, double time)
        {
            GPSSec = time;
            Attitude = attitude;
            BLH = bLHCoordinate;
            Velocity = velocity;
            double sinB = Math.Sin(BLH.B);
            double eSinB2 = Math.Pow(Constant.GRS80.Eccentricity_1 * sinB, 2);
            R_M = Constant.GRS80.Semi_major * (1 - Math.Pow(Constant.GRS80.Eccentricity_1, 2)) / Math.Pow(1 - eSinB2, 1.5);
            R_N = Constant.GRS80.Semi_major / Math.Sqrt(1 - eSinB2);
            Omega_ie = new Vector3d(Constant.GRS80.EarthRotationAngleVelocity * Math.Cos(BLH.B), 0, -Constant.GRS80.EarthRotationAngleVelocity * sinB);
            Omega_en = new Vector3d(Velocity[1] / (R_N + BLH.H), -Velocity[0] / (R_M + BLH.H), -Velocity[1] * Math.Tan(BLH.B) / (R_N + BLH.H));
            double g0 = 9.7803267715 * (1 + 0.0052790414 * Math.Pow(sinB, 2) + 0.0000232718 * Math.Pow(sinB, 4));
            Gravity = new Vector3d(0, 0, g0 - (3.087691089e-6 - 4.397731e-9 * Math.Pow(sinB, 2)) * BLH.H + 0.721e-12 * BLH.H * BLH.H);
        }

        public MotionState()
        {
        }

        public MotionState Clone()
        {
            return new(this.Attitude.Clone(), this.BLH.Clone(), this.Velocity.Clone(), this.GPSSec);
        }

    }
}
