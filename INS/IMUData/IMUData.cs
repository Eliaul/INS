using System;

namespace INS
{
    internal class IMUData
    {
        public static readonly double acc_scale = 0.05 / Math.Pow(2, 15);
        public static readonly double gyro_scale = 0.1 / 3600.0 / Math.Pow(2, 8);
        public static double samplingRate;

        public double GPSsec { get; set; }
        public Vector3d DeltaAngle { get; set; } = default!;
        public Vector3d DeltaVelocity { get; set; } = default!;

        public IMUData(double gPSsec, Vector3d deltaAngle, Vector3d deltaVelocity)
        {
            GPSsec = gPSsec;
            DeltaAngle = deltaAngle;
            DeltaVelocity = deltaVelocity;
            //GPSsec = gPSsec;
            //DeltaAngle = deltaAngle.Clone();
            //DeltaVelocity = deltaVelocity.Clone();
        }

        public void SetValue(double gPSsec, Vector3d deltaAngle, Vector3d deltaVelocity)
        {
            GPSsec = gPSsec;
            DeltaAngle.SetValue(deltaAngle[0], deltaAngle[1], deltaAngle[2]);
            DeltaVelocity.SetValue(deltaVelocity[0], deltaVelocity[1], deltaVelocity[2]);
        }

        public IMUData Clone()
        {
            IMUData data = new();
            data.SetValue(GPSsec, DeltaAngle, DeltaVelocity);
            return data;
        }

        public IMUData()
        {
            DeltaAngle = new();
            DeltaVelocity = new();  
        }
    }
}
