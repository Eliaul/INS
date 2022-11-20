

namespace INS
{
    enum CoordinateSystem
    {
        CGCS2000,
        WGS84,
        GRS80,
    }

    internal class Ellipsoid
    {
        /// <summary>
        /// 短半轴
        /// </summary>
        public double Semi_minor { get; }
        /// <summary>
        /// 长半轴
        /// </summary>
        public double Semi_major { get; }
        /// <summary>
        /// 扁率
        /// </summary>
        public double Oblateness { get; }
        /// <summary>
        /// 第一偏心率
        /// </summary>
        public double Eccentricity_1 { get; }
        /// <summary>
        /// 第二偏心率
        /// </summary>
        public double Eccentricity_2 { get; }
        /// <summary>
        /// 地球自转角速度
        /// </summary>
        public double EarthRotationAngleVelocity { get; }

        public Ellipsoid(CoordinateSystem coordinateSystem)
        {
            switch (coordinateSystem)
            {
                case CoordinateSystem.CGCS2000:
                    Semi_minor = 6356752.3141403558;
                    Semi_major = 6378137;
                    Oblateness = 1.0 / 298.2572221010042;
                    Eccentricity_1 = 0.081819191042811;
                    Eccentricity_2 = 0.0820944381519236;
                    EarthRotationAngleVelocity = 7.292115e-5;
                    break;
                case CoordinateSystem.WGS84:
                    Semi_minor = 6356752.3142451795;
                    Semi_major = 6378137;
                    Oblateness = 1.0 / 298.2572235629972;
                    Eccentricity_1 = 0.0818191908425524;
                    Eccentricity_2 = 0.0820944379496565;
                    EarthRotationAngleVelocity = 7.292115e-5;
                    break;
                case CoordinateSystem.GRS80:
                    Semi_minor = 6356752.3141;
                    Semi_major = 6378137;
                    Oblateness = 1.0 / 298.2572221008827;
                    Eccentricity_1 = 0.0818191910428318;
                    Eccentricity_2 = 0.0820944381519334;
                    EarthRotationAngleVelocity = 7.292115e-5;
                    break;
            }
        }
    }
}
