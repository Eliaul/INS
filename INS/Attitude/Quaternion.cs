namespace INS
{
    internal partial class Quaternion
    {
        private readonly double[] q = new double[4];

        public Quaternion(double q0, double q1, double q2, double q3)
        {
            this.q[0] = q0;
            this.q[1] = q1;
            this.q[2] = q2;
            this.q[3] = q3;
        }

        public Quaternion() { }

        public Quaternion Clone()
        {
            return new(this.q[0], this.q[1], this.q[2], this.q[3]);
        }

        public Quaternion(Vector4d v)
        {
            for (int i = 0; i < 4; i++)
            {
                this.q[i] = v[i];
            }
        }

        public static Quaternion Identity
        {
            get { return new Quaternion(1, 0, 0, 0); }
        }

        public double this[int i]
        {
            get { if (i < 0 || i > 3) throw new ArgumentException("索引超限"); return q[i]; }
            set { if (i < 0 || i > 3) throw new ArgumentException("索引超限"); q[i] = value; }
        }

        public void SetValue(double q0, double q1, double q2, double q3)
        {
            q[0] = q0;
            q[1] = q1;
            q[2] = q2;
            q[3] = q3;
        }

        #region "运算符重载"
        public static Quaternion operator *(Quaternion quaternion, double num)
        {
            Quaternion res = new();
            for (int i = 0; i < 4; i++)
            {
                res.q[i] = quaternion.q[i] * num;
            }
            return res;
        }

        public static Quaternion operator *(double num, Quaternion quaternion)
        {
            return quaternion * num;
        }

        public static Quaternion operator *(Quaternion p, Quaternion q)
        {
            return new Quaternion(p[0] * q[0] - p[1] * q[1] - p[2] * q[2] - p[3] * q[3], p[0] * q[1] + p[1] * q[0] + p[2] * q[3] - p[3] * q[2],
                p[0] * q[2] + p[2] * q[0] + p[3] * q[1] - p[1] * q[3], p[0] * q[3] + p[3] * q[0] + p[1] * q[2] - p[2] * q[1]);
        }

        public static Quaternion operator /(Quaternion quaternion, double num)
        {
            return quaternion * (1.0 / num);
        }
            
        public static Quaternion operator -(Quaternion quaternion)
        {
            Quaternion res = new();
            for (int i = 0; i < 4; i++)
            {
                res.q[i] = -quaternion[i];
            }
            return res;
        }

        public static Quaternion operator +(Quaternion quaternion1, Quaternion quaternion2)
        {
            Quaternion res = new();
            for (int i = 0; i < 4; i++)
            {
                res.q[i] = quaternion1[i] + quaternion2[i];
            }
            return res;
        }

        public static Quaternion operator -(Quaternion quaternion1, Quaternion quaternion2)
        {
            return quaternion1 + (-quaternion2);
        }
        #endregion

        public override string ToString()
        {
            return String.Format("{0,12:F4}{1,12:F4}i{2,12:F4}j{3,12:F4}k", this.q[0], this.q[1], this.q[2], this.q[3]);
        }

        public Vector3d ImaginaryPart()
        {
            return new Vector3d(this.q[1], this.q[2], this.q[3]);
        }
    }

    #region "四元数的静态方法"
    internal partial class Quaternion
    {
        public static double Norm(Quaternion quaternion)
        {
            double res = 0;
            for (int i = 0; i < 4; i++)
            {
                res += quaternion.q[i] * quaternion.q[i];
            }
            return Math.Sqrt(res);
        }

        public static Quaternion Normalize(Quaternion quaternion) => quaternion / Norm(quaternion);

        public static Quaternion Conjugate(Quaternion quaternion) => new(quaternion[0], -quaternion[1], -quaternion[2], -quaternion[3]);

        public static Quaternion Inverse(Quaternion quaternion) => Conjugate(quaternion) / Math.Pow(Norm(quaternion), 2);

        public static Quaternion UnitQuaternionFromAxisAngle(Vector3d axis, double angle)
        {
            Quaternion res = new();
            double halfAngle = angle * 0.5;
            double s = Math.Sin(halfAngle);
            double c = Math.Cos(halfAngle);
            res.q[0] = c;
            Vector3d unitAxis = VectorOperate.Unitize(axis);
            for (int i = 1; i < 4; i++)
            {
                res.q[i] = unitAxis[i - 1] * s;
            }
            return res;
        }

        public static Quaternion FromRotationVector(Vector3d vector)
        {
            double norm = VectorOperate.Norm(vector);
            if (norm < 1e-14)
            {
                return new(1, 0, 0, 0);
            }
            double halfAngle = norm * 0.5;
            double s = Math.Sin(halfAngle);
            double c = Math.Cos(halfAngle);
            Vector3d unitVector = VectorOperate.Unitize(vector);
            return new(c, unitVector[0] * s, unitVector[1] * s, unitVector[2] * s);
        }

        public static Vector3d ToRotationVector(Quaternion quaternion)
        {
            if (Math.Abs(Norm(quaternion) - 1) > 1e-10)
            {
                throw new ArgumentException("这不是单位四元数");
            }
            if (Math.Abs(quaternion.q[0]) < 1e-10)
            {
                return new Vector3d(quaternion[1], quaternion[2], quaternion[3]) * Math.PI;
            }
            double theta = 2 * Math.Atan2(VectorOperate.Norm(quaternion.ImaginaryPart()), Math.Abs(quaternion.q[0]));
            return VectorOperate.Unitize(quaternion.ImaginaryPart()) * theta;
        }

        public static EulerAngle ToEulerAngle(Quaternion quaternion)
        {
            double q0 = quaternion.q[0];
            double q1 = quaternion.q[1];
            double q2 = quaternion.q[2];
            double q3 = quaternion.q[3];
            return new EulerAngle(new Matrix3d(new double[,] {
                {q0 * q0 + q1 * q1 - q2 * q2 - q3 * q3, 2 * (q1 * q2 - q0 * q3), 2 * (q1 * q3 + q0 * q2) },
                {2 * (q1 * q2 + q0 * q3), q0 * q0 - q1 * q1 + q2 * q2 - q3 * q3, 2 * (q2 * q3 - q0 * q1) },
                {2 * (q1 * q3 - q0 * q2), 2 * (q2 * q3 + q0 * q1), q0 * q0 - q1 * q1 - q2 * q2 + q3 * q3 }
            }));
        }

        public static Matrix3d ToRotationMatrix(Quaternion quaternion)
        {
            double q0 = quaternion.q[0];
            double q1 = quaternion.q[1];
            double q2 = quaternion.q[2];
            double q3 = quaternion.q[3];
            return new Matrix3d(new double[,] {
                {q0 * q0 + q1 * q1 - q2 * q2 - q3 * q3, 2 * (q1 * q2 - q0 * q3), 2 * (q1 * q3 + q0 * q2) },
                {2 * (q1 * q2 + q0 * q3), q0 * q0 - q1 * q1 + q2 * q2 - q3 * q3, 2 * (q2 * q3 - q0 * q1) },
                {2 * (q1 * q3 - q0 * q2), 2 * (q2 * q3 + q0 * q1), q0 * q0 - q1 * q1 - q2 * q2 + q3 * q3 }
            });
        }
    }
    #endregion
}
