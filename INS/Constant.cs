

namespace INS
{
    internal static class Constant
    {
        public static readonly Ellipsoid WGS84 = new(CoordinateSystem.WGS84);
        public static readonly Ellipsoid GRS80 = new(CoordinateSystem.GRS80);
    }
}
