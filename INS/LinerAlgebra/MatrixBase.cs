using System.Text;

namespace INS
{
    /// <summary>
    /// 矩阵基类,无法实例化,利用CRTP实现通用运算符重载
    /// </summary>
    /// <typeparam name="Derived">向量或者矩阵</typeparam>
    internal abstract class MatrixBase<Derived> where Derived : MatrixBase<Derived>
    {
        protected double[,] _elements = default!;
        protected int _row;
        protected int _col;

        public int Row => _row;

        public int Col => _col;

        public abstract Derived Clone();

        public abstract Derived CloneSize();

        #region "运算符重载"
        public static Derived operator +(MatrixBase<Derived> m1, MatrixBase<Derived> m2)
        {
            Derived res = m1.CloneSize();
            for (int i = 0; i < m1.Row; i++)
            {
                for (int j = 0; j < m1.Col; j++)
                {
                    res._elements[i, j] = m1._elements[i, j] + m2._elements[i, j];
                }
            }
            return res;
        }

        public static Derived operator -(MatrixBase<Derived> m)
        {
            Derived res = m.CloneSize();
            for (int i = 0; i < m._row; i++)
            {
                for (int j = 0; j < m._col; j++)
                {
                    res._elements[i, j] = -m._elements[i, j];
                }
            }
            return res;
        }

        public static Derived operator -(MatrixBase<Derived> m1, MatrixBase<Derived> m2)
        {
            return m1 + -m2;
        }

        public static Derived operator *(MatrixBase<Derived> m, double num)
        {
            Derived res = m.CloneSize();
            for (int i = 0; i < m._row; i++)
            {
                for (int j = 0; j < m._col; j++)
                {
                    res._elements[i, j] = m._elements[i, j] * num;
                }
            }
            return res;
        }

        public static Derived operator *(double num, MatrixBase<Derived> m)
        {
            return m * num;
        }

        public static Derived operator /(MatrixBase<Derived> m, double num)
        {
            return m * (1.0 / num);
        }
        #endregion

        public override string ToString()
        {
            StringBuilder stringBuilder = new();
            for (int i = 0; i < _row; i++)
            {
                for (int j = 0; j < _col; j++)
                {
                    stringBuilder.Append(string.Format("{0,12:F4}", _elements[i, j]));
                }
                stringBuilder.Append("\r\n");
            }
            stringBuilder.Remove(stringBuilder.Length - 2, 2);
            return stringBuilder.ToString();
        }
    }
}
