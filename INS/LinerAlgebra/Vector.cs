
namespace INS
{
    interface IVector<Derived> where Derived : IVector<Derived>
    {

        static abstract Derived Zeros(int length);

        static abstract Derived Constant(double constant, int length);

        static abstract Derived Random(int length);

        static abstract double Dot(Derived v1, Derived v2);

        static abstract double Norm(Derived v);

        static abstract double Distance(Derived v1, Derived v2);

        static abstract Derived Unitize(Derived v);
    }

    internal partial class Vector : MatrixBase<Vector>, IVector<Vector>
    {
        #region "构造函数"
        public Vector(int row)
        {
            this._row = row;
            this._col = 1;
            this._elements = new double[_row, 1];
        }

        public Vector(double[] doubles)
        {
            this._row = doubles.Length;
            this._col = 1;
            this._elements = new double[doubles.Length, 1];
            for (int i = 0; i < doubles.Length; i++)
            {
                this._elements[i, 0] = doubles[i];
            }
        }
        #endregion

        public override Vector Clone()
        {
            Vector res = new(_row)
            {
                _elements = (double[,])this._elements.Clone()
            };
            return res;
        }

        public override Vector CloneSize() => new(_row);

        public double this[int i]
        {
            get { return this._elements[i, 0]; }
            set { this._elements[i, 0] = value; }
        }
    }

    internal partial class Vector3d : MatrixBase<Vector3d>, IVector<Vector3d>
    {
        #region "构造函数"
        public Vector3d()
        {
            this._row = 3;
            this._col = 1;
            this._elements = new double[3, 1];
        }
        public Vector3d(double x, double y, double z)
        {
            this._row = 3;
            this._col = 1;
            this._elements = new double[3, 1] { { x }, { y }, { z } };
        }
        public Vector3d(double[] doubles)
        {
            if (doubles.Length != 3)
            {
                throw new ArgumentException("元素的个数不为3个");
            }
            this._row = 3;
            this._col = 1;
            this._elements = new double[3, 1] { { doubles[0] }, { doubles[1] }, { doubles[2] } };
        }
        #endregion

        public double this[int i]
        {
            get { return this._elements[i, 0]; }
            set { this._elements[i, 0] = value; }
        }

        public override Vector3d Clone()
        {
            Vector3d res = new()
            {
                _elements = (double[,])this._elements.Clone()
            };
            return res;
        }

        public override Vector3d CloneSize() => new();

        public Matrix3d SkewSymmetricMatrix() => new(new double[,] {
                {0, -this[2], this[1] },
                {this[2], 0, -this[0] },
                {-this[1], this[0], 0 }
            });

        public void SetValue(double x, double y, double z)
        {
            this._elements[0, 0] = x;
            this._elements[1, 0] = y;
            this._elements[2, 0] = z;
        }
    }

    internal partial class Vector4d : MatrixBase<Vector4d>, IVector<Vector4d>
    {
        #region "构造函数"
        public Vector4d()
        {
            this._row = 4;
            this._col = 1;
            this._elements = new double[4, 1];
        }

        public Vector4d(double x, double y, double z, double w)
        {
            this._row = 4;
            this._col = 1;
            this._elements = new double[4, 1] { { x }, { y }, { z }, { w } };
        }
        #endregion

        public double this[int i]
        {
            get { return this._elements[i, 0]; }
            set { this._elements[i, 0] = value; }
        }

        public override Vector4d Clone()
        {
            Vector4d res = new()
            {
                _elements = (double[,])this._elements.Clone()
            };
            return res;
        }

        public override Vector4d CloneSize() => new();
    }
}

#region "Vector有关的类型转换"
namespace INS
{
    internal partial class Vector : MatrixBase<Vector>, IVector<Vector>
    {
        public static implicit operator Vector(Vector3d v) => new(new double[] { v[0], v[1], v[2] });

        public static implicit operator Vector(Vector4d v) => new(new double[] { v[0], v[1], v[2], v[3] });

        public static explicit operator Vector(Matrix m)
        {
            if (m.Col != 1)
            {
                throw new ArgumentException("转换失败");
            }
            Vector res = new(m.Row);
            for (int i = 0; i < res.Row; i++)
            {
                res._elements[i, 0] = m[i, 0];
            }
            return res;
        }
    }

    internal partial class Vector3d : MatrixBase<Vector3d>, IVector<Vector3d>
    {
        public static explicit operator Vector3d(Vector v)
        {
            if (v.Row != 3)
            {
                throw new ArgumentException("转换失败");
            }
            return new(v[0], v[1], v[2]);
        }
    }

    internal partial class Vector4d : MatrixBase<Vector4d>, IVector<Vector4d>
    {
        public static explicit operator Vector4d(Vector v)
        {
            if (v.Row != 4)
            {
                throw new ArgumentException("转换失败");
            }
            return new(v[0], v[1], v[2], v[3]);
        }
    }
}
#endregion

#region "Vector有关的静态方法"
namespace INS
{
    internal partial class Vector : MatrixBase<Vector>, IVector<Vector>
    {
        public static Vector Zeros(int length)
        {
            Vector res = new(length);
            for (int i = 0; i < length; i++)
            {
                res._elements[i, 0] = 0;
            }
            return res;
        }

        public static Vector Constant(double constant, int length)
        {
            Vector res = new(length);
            for (int i = 0; i < length; i++)
            {
                res._elements[i, 0] = constant;
            }
            return res;
        }

        public static Vector Random(int length)
        {
            Vector res = new(length);
            Random r = new();
            for (int i = 0; i < res._row; i++)
            {
                res._elements[i, 0] = r.NextDouble();
            }
            return res;
        }

        public static double Dot(Vector v1, Vector v2)
        {
            if (v1._row != v2._row)
            {
                throw new ArgumentException("两个向量长度不同");
            }
            double sum = 0;
            for (int i = 0; i < v1._row; i++)
            {
                sum += v1._elements[i, 0] * v2._elements[i, 0];
            }
            return sum;
        }

        public static double Norm(Vector v)
        {
            double sum = 0;
            for (int i = 0; i < v._row; i++)
            {
                sum += v._elements[i, 0] * v._elements[i, 0];
            }
            return Math.Sqrt(sum);
        }

        public static double Distance(Vector v1, Vector v2) => Norm(v1 - v2);

        public static Vector Unitize(Vector v) => v / Norm(v);
    }

    internal partial class Vector3d : MatrixBase<Vector3d>, IVector<Vector3d>
    {
        public static Vector3d Zeros(int length = 3) => new(0, 0, 0);

        public static Vector3d Constant(double constant, int length = 3) => new(constant, constant, constant);

        public static Vector3d Random(int length = 3)
        {
            Vector3d res = new();
            Random r = new();
            for (int i = 0; i < 3; i++)
            {
                res._elements[i, 0] = r.NextDouble();
            }
            return res;
        }

        public static double Dot(Vector3d v1, Vector3d v2)
        {
            double sum = 0;
            for (int i = 0; i < 3; i++)
            {
                sum += v1._elements[i, 0] * v2._elements[i, 0];
            }
            return sum;
        }

        public static Vector3d CrossProduct(Vector3d v1, Vector3d v2)
        {
            double x1 = v1._elements[0, 0];
            double y1 = v1._elements[1, 0];
            double z1 = v1._elements[2, 0];
            double x2 = v2._elements[0, 0];
            double y2 = v2._elements[1, 0];
            double z2 = v2._elements[2, 0];
            return new Vector3d(-y2 * z1 + y1 * z2, x2 * z1 - x1 * z2, -x2 * y1 + x1 * y2);
        }

        public static double Norm(Vector3d v)
        {
            double sum = 0;
            for (int i = 0; i < 3; i++)
            {
                sum += v._elements[i, 0] * v._elements[i, 0];
            }
            return Math.Sqrt(sum);
        }

        public static double Distance(Vector3d v1, Vector3d v2) => Norm(v1 - v2);

        public static Vector3d Unitize(Vector3d v) => v / Norm(v);
    }

    internal partial class Vector4d : MatrixBase<Vector4d>, IVector<Vector4d>
    {
        public static Vector4d Zeros(int length = 4) => new(0, 0, 0, 0);

        public static Vector4d Constant(double constant, int length = 4) => new(constant, constant, constant, constant);

        public static Vector4d Random(int length = 4)
        {
            Vector4d res = new();
            Random r = new();
            for (int i = 0; i < 4; i++)
            {
                res._elements[i, 0] = r.NextDouble();
            }
            return res;
        }

        public static double Dot(Vector4d v1, Vector4d v2)
        {
            double sum = 0;
            for (int i = 0; i < 4; i++)
            {
                sum += v1._elements[i, 0] * v2._elements[i, 0];
            }
            return sum;
        }

        public static double Norm(Vector4d v)
        {
            double sum = 0;
            for (int i = 0; i < 4; i++)
            {
                sum += v._elements[i, 0] * v._elements[i, 0];
            }
            return Math.Sqrt(sum);
        }

        public static double Distance(Vector4d v1, Vector4d v2) => Norm(v1 - v2);

        public static Vector4d Unitize(Vector4d v) => v / Norm(v);
    }
}
#endregion