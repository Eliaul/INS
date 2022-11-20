
namespace INS
{
    interface IMatrix<Derived> where Derived : IMatrix<Derived>
    {
        static abstract Derived Transpose(Derived m);

        static abstract double Trace(Derived m);

        static abstract Derived Zeros(int row, int col);

        static abstract Derived Identity(int rank);

        static abstract Derived Random(int row, int col);

        static abstract Derived Inverse(Derived m);
    }


    internal partial class Matrix : MatrixBase<Matrix>, IMatrix<Matrix>
    {
        #region "构造函数"
        public Matrix(int row, int col)
        {
            this._row = row;
            this._col = col;
            this._elements = new double[_row, col];
        }

        public Matrix(double[,] doubles)
        {
            this._row = doubles.GetLength(0);
            this._col = doubles.GetLength(1);
            this._elements = new double[_row, _col];
            for (int i = 0; i < _row; i++)
            {
                for (int j = 0; j < _col; j++)
                {
                    this._elements[i, j] = doubles[i, j];
                }
            }
        }
        #endregion

        public override Matrix Clone()
        {
            Matrix res = new(_row, _col)
            {
                _elements = (double[,])this._elements.Clone()
            };
            return res;
        }

        public override Matrix CloneSize() => new(_row, _col);

        public double this[int i, int j]
        {
            get { return _elements[i, j]; }
            set { _elements[i, j] = value; }
        }
    }

    internal partial class Matrix3d : MatrixBase<Matrix3d>, IMatrix<Matrix3d>
    {
        #region "构造函数"
        public Matrix3d()
        {
            this._col = 3;
            this._row = 3;
            this._elements = new double[3, 3];
        }

        public Matrix3d(double[,] doubles)
        {
            if (doubles.GetLength(0) != 3 || doubles.GetLength(1) != 3)
            {
                throw new ArgumentException("数组元素不为9个");
            }
            this._row = 3;
            this._col = 3;
            this._elements = new double[3, 3];
            for (int i = 0; i < _row; i++)
            {
                for (int j = 0; j < _col; j++)
                {
                    this._elements[i, j] = doubles[i, j];
                }
            }
        }
        #endregion

        public double this[int i, int j]
        {
            get { return _elements[i, j]; }
            set { _elements[i, j] = value; }
        }

        public override Matrix3d Clone()
        {
            Matrix3d res = new()
            {
                _elements = (double[,])this._elements.Clone()
            };
            return res;
        }

        public override Matrix3d CloneSize() => new();


    }
}

#region "矩阵有关的类型转换"
namespace INS
{
    internal partial class Matrix : MatrixBase<Matrix>, IMatrix<Matrix>
    {
        public static implicit operator Matrix(Vector v)
        {
            Matrix res = new(v.Row, 1);
            for (int i = 0; i < v.Row; i++)
            {
                res._elements[i, 0] = v[i];
            }
            return res;
        }

        public static implicit operator Matrix(Matrix3d m)
        {
            Matrix res = new(3, 3);
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    res._elements[i, j] = m[i, j];
                }
            }
            return res;
        }
    }

    internal partial class Matrix3d : MatrixBase<Matrix3d>, IMatrix<Matrix3d>
    {
        public static implicit operator Matrix3d(Vector3d v) => new(new double[,] { { v[0] }, { v[1] }, { v[2] } });

        public static explicit operator Matrix3d(Matrix m)
        {
            if (m.Col != 3 || m.Row != 3)
            {
                throw new ArgumentException("转换失败");
            }
            Matrix3d res = new();
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    res._elements[i, j] = m[i, j];
                }
            }
            return res;
        }
    }
}
#endregion

#region "矩阵的专有运算"
namespace INS
{
    internal partial class Matrix : MatrixBase<Matrix>, IMatrix<Matrix>
    {
        public static Matrix Transpose(Matrix m)
        {
            Matrix res = new(m._col, m._row);
            for (int i = 0; i < res._row; i++)
            {
                for (int j = 0; j < res._col; j++)
                {
                    res._elements[i, j] = m._elements[j, i];
                }
            }
            return res;
        }

        public static Matrix operator *(Matrix m1, Matrix m2)
        {
            if (m1._col != m2._row)
            {
                throw new ArgumentException("这两个矩阵不能相乘");
            }
            Matrix res = new(m1._row, m2._col);
            for (int i = 0; i < m1._row; i++)
            {
                for (int j = 0; j < m2._col; j++)
                {
                    res._elements[i, j] = 0;
                    for (int k = 0; k < m1._col; k++)
                    {
                        res._elements[i, j] += m1._elements[i, k] * m2._elements[k, j];
                    }
                }
            }
            return res;
        }

        public static Vector operator *(Matrix m, Vector v)
        {
            if (m._col != v.Row)
            {
                throw new ArgumentException("无法相乘");
            }
            Vector res = new(m._row);
            for (int i = 0; i < m._row; i++)
            {
                double num = 0;
                for (int j = 0; j < m._col; j++)
                {
                    num += m._elements[i, j] * v[j];
                }
                res[i] = num;
            }
            return res;
        }

        public static double Trace(Matrix m)
        {
            if (m._row != m._col)
            {
                throw new ArgumentException("该矩阵不是方阵");
            }
            double num = 0;
            for (int i = 0; i < m._row; i++)
            {
                num += m._elements[i, i];
            }
            return num;
        }

        public static Matrix Zeros(int row, int col)
        {
            Matrix res = new(row, col);
            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < col; j++)
                {
                    res._elements[i, j] = 0;
                }
            }
            return res;
        }

        public static Matrix Identity(int rank)
        {
            Matrix res = new(rank, rank);
            for (int i = 0; i < rank; i++)
            {
                for (int j = 0; j < rank; j++)
                {
                    res._elements[i, j] = (i != j) ? 0 : 1;
                }
            }
            return res;
        }

        public static Matrix Random(int row, int col)
        {
            Matrix res = new(row, col);
            Random r = new();
            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < col; j++)
                {
                    res._elements[i, j] = r.NextDouble();
                }
            }
            return res;
        }
    }

    internal partial class Matrix3d : MatrixBase<Matrix3d>, IMatrix<Matrix3d>
    {
        public static Matrix3d Transpose(Matrix3d m)
        {
            Matrix3d res = new();
            for (int i = 0; i < res._row; i++)
            {
                for (int j = 0; j < res._col; j++)
                {
                    res._elements[i, j] = m._elements[j, i];
                }
            }
            return res;
        }

        public static Matrix3d operator *(Matrix3d m1, Matrix3d m2)
        {
            Matrix3d res = new();
            for (int i = 0; i < m1._row; i++)
            {
                for (int j = 0; j < m2._col; j++)
                {
                    res._elements[i, j] = 0;
                    for (int k = 0; k < m1._col; k++)
                    {
                        res._elements[i, j] += m1._elements[i, k] * m2._elements[k, j];
                    }
                }
            }
            return res;
        }

        public static Vector3d operator *(Matrix3d m, Vector3d v)
        {
            Vector3d res = new();
            for (int i = 0; i < m._row; i++)
            {
                double num = 0;
                for (int j = 0; j < m._col; j++)
                {
                    num += m._elements[i, j] * v[j];
                }
                res[i] = num;
            }
            return res;
        }

        public static double Trace(Matrix3d m) => m[0, 0] + m[1, 1] + m[2, 2];

        public static Matrix3d Zeros(int row = 3, int col = 3)
        {
            Matrix3d res = new();
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    res._elements[i, j] = 0;
                }
            }
            return res;
        }

        public static Matrix3d Identity(int rank = 3)
        {
            Matrix3d res = new();
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    res._elements[i, j] = (i != j) ? 0 : 1;
                }
            }
            return res;
        }

        public static Matrix3d Random(int row = 3, int col = 3)
        {
            Matrix3d res = new();
            Random r = new();
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    res._elements[i, j] = r.NextDouble();
                }
            }
            return res;
        }
    }
}
#endregion

#region "矩阵求逆"
namespace INS
{
    internal partial class Matrix : MatrixBase<Matrix>, IMatrix<Matrix>
    {
        #region "初等行变换"
        private Matrix RowExchange(int row1, int row2)
        {
            for (int i = 0; i < this._col; i++)
            {
                (this._elements[row2, i], this._elements[row1, i]) = (this._elements[row1, i], this._elements[row2, i]);
            }
            return this;
        }

        private Matrix RowMultiply(int row, double multiple)
        {
            for (int i = 0; i < this._col; i++)
            {
                this._elements[row, i] *= multiple;
            }
            return this;
        }

        private Matrix Row1MultiplyAddToRow2(int row1, int row2, double multiple)
        {
            for (int i = 0; i < this._col; i++)
            {
                this._elements[row2, i] += this._elements[row1, i] * multiple;
            }
            return this;
        }
        #endregion

        public static Matrix Inverse(Matrix m)
        {
            if (m._row != m._col)
            {
                throw new Exception("该矩阵不是方阵");
            }

            Matrix unitMatrix = Matrix.Identity(m._row);
            Matrix cpy = m.Clone();

            for (int col = 0; col < m._col; col++)
            {
                for (int row = col; row < m._row; row++)
                {
                    if (!double.Equals(Math.Abs(cpy._elements[row, col]), 0)) //不等于0
                    {
                        cpy.RowExchange(row, col);   //将第col+1列不为0的元素移到第col+1行第col+1列
                        unitMatrix.RowExchange(row, col);   //单位阵做同样的操作
                        break;
                    }
                    if (row == cpy._row - 1)
                    {
                        throw new Exception("该矩阵不可逆");
                    }
                }

                unitMatrix.RowMultiply(col, 1.0 / cpy._elements[col, col]);
                cpy.RowMultiply(col, 1.0 / cpy._elements[col, col]);

                for (int i = 0; i < col; i++)
                {
                    unitMatrix.Row1MultiplyAddToRow2(col, i, -cpy._elements[i, col]);
                    cpy.Row1MultiplyAddToRow2(col, i, -cpy._elements[i, col]);
                }

                for (int i = col + 1; i < cpy._row; i++)
                {
                    unitMatrix.Row1MultiplyAddToRow2(col, i, -cpy._elements[i, col]);
                    cpy.Row1MultiplyAddToRow2(col, i, -cpy._elements[i, col]);
                }
            }
            return unitMatrix;
        }
    }

    internal partial class Matrix3d : MatrixBase<Matrix3d>, IMatrix<Matrix3d>
    {
        public static Matrix3d Inverse(Matrix3d m)
        {
            double a1 = m[0, 0];
            double a2 = m[1, 0];
            double a3 = m[2, 0];
            double b1 = m[0, 1];
            double b2 = m[1, 1];
            double b3 = m[2, 1];
            double c1 = m[0, 2];
            double c2 = m[1, 2];
            double c3 = m[2, 2];
            double num = a1 * (b2 * c3 - c2 * b3) - a2 * (b1 * c3 - c1 * b3) + a3 * (b1 * c2 - c1 * b2);
            double[,] doubles = new double[3, 3] {
                {b2 * c3 - c2 * b3, c1 * b3 - b1 * c3, b1 * c2 - c1 * b2 },
                {c2 * a3 - a2 * c3, a1 * c3 - c1 * a3, a2 * c1 - a1 * c2 },
                {a2 * b3 - b2 * a3, b1 * a3 - a1 * b3, a1 * b2 - a2 * b1 }
            };
            return new Matrix3d(doubles) / num;
        }
    }
}
#endregion