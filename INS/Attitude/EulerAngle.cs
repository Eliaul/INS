
namespace INS
{
    internal partial class EulerAngle
    {
        /// <summary>
        /// 横滚角(phi)
        /// </summary>
        private double _roll = default!;

        /// <summary>
        /// 俯仰角(theta)
        /// </summary>
        private double _pitch = default!;

        /// <summary>
        /// 航向角(psi)
        /// </summary>
        private double _yaw = default!;

        /// <summary>
        /// C_b^R
        /// </summary>
        private readonly Matrix3d _rotationMatrix = default!;

        public double Roll
        { get { return _roll; } set { _roll = value; } }

        public double Pitch
        { get { return _pitch; } set { _pitch = value; } }

        public double Yaw
        { get { return _yaw; } set { _yaw = value; } }

        public Matrix3d RotationMatrix
        { get { return _rotationMatrix; } }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="roll">横滚角(rad)</param>
        /// <param name="pitch">俯仰角(rad)</param>
        /// <param name="yaw">航向角(rad)</param>
        public EulerAngle(double roll, double pitch, double yaw)
        {
            this._roll = roll;
            this._pitch = pitch;
            this._yaw = yaw;

            double cosRoll = Math.Cos(_roll);
            double sinRoll = Math.Sin(_roll);
            double cosPitch = Math.Cos(_pitch);
            double sinPitch = Math.Sin(_pitch);
            double cosYaw = Math.Cos(_yaw);
            double sinYaw = Math.Sin(_yaw);
            _rotationMatrix = new Matrix3d(new double[,] {
                { cosPitch * cosYaw, -cosRoll * sinYaw + sinRoll * sinPitch * cosYaw, sinRoll * sinYaw + cosRoll * sinPitch * cosYaw},
                {cosPitch * sinYaw, cosRoll * cosYaw + sinRoll * sinPitch * sinYaw, -sinRoll * cosYaw + cosRoll * sinPitch * sinYaw },
                {-sinPitch, sinRoll * cosPitch, cosRoll * cosPitch }
            });
        }

        public EulerAngle(Matrix3d rotationMatrix)
        {
            _rotationMatrix = rotationMatrix.Clone();
            _pitch = -Math.Asin(rotationMatrix[2, 0]);
            if (Math.Abs(rotationMatrix[2, 0]) < 0.9999999)
            {
                _yaw = Math.Atan2(rotationMatrix[1, 0], rotationMatrix[0, 0]);
                _roll = Math.Atan2(rotationMatrix[2, 1], rotationMatrix[2, 2]);
            }
            else
            {
                throw new ArgumentException("横滚角为90°");
            }
        }

        public double this[int i]
        {
            get { if (i == 0) return Angle.Rad2Deg(_roll); if (i == 1) return Angle.Rad2Deg(_pitch); if (i == 2) return Angle.Rad2Deg(_yaw); throw new ArgumentException("索引超出界限"); }
        }

        public void SetValue(double roll, double pitch, double yaw)
        {
            _roll = roll;
            _pitch = pitch;
            _yaw = yaw;
        }

        public EulerAngle()
        { }
    }

    internal partial class EulerAngle
    {
        public static Quaternion ToQuaternion(EulerAngle eulerAngle)
        {
            double cosHalfRoll = Math.Cos(eulerAngle._roll / 2.0);
            double sinHalfRoll = Math.Sin(eulerAngle._roll / 2.0);
            double cosHalfPitch = Math.Cos(eulerAngle._pitch / 2.0);
            double sinHalfPitch = Math.Sin(eulerAngle._pitch / 2.0);
            double cosHalfYaw = Math.Cos(eulerAngle._yaw / 2.0);
            double sinHalfYaw = Math.Sin(eulerAngle._yaw / 2.0);
            return new Quaternion(
                cosHalfRoll * cosHalfPitch * cosHalfYaw + sinHalfRoll * sinHalfPitch * sinHalfYaw,
                sinHalfRoll * cosHalfPitch * cosHalfYaw - cosHalfRoll * sinHalfPitch * sinHalfYaw,
                cosHalfRoll * sinHalfPitch * cosHalfYaw + sinHalfRoll * cosHalfPitch * sinHalfYaw,
                cosHalfRoll * cosHalfPitch * sinHalfYaw - sinHalfRoll * sinHalfPitch * cosHalfYaw
                );
        }
    }
}