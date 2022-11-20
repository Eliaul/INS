namespace INS
{
    /// <summary>
    /// 利用接口封装的矩阵运算类
    /// </summary>
    internal static class MatrixOperate
    {
        public static T Transpose<T>(T m) where T : IMatrix<T> => T.Transpose(m);
        public static double Trace<T>(T m) where T : IMatrix<T> => T.Trace(m);
        public static T Inverse<T>(T m) where T : IMatrix<T> => T.Inverse(m);
    }

    /// <summary>
    /// 利用接口封装的向量运算类
    /// </summary>
    internal static class VectorOperate
    {
        public static double Dot<T>(T v1, T v2) where T : IVector<T> => T.Dot(v1, v2);
        public static double Norm<T>(T v) where T : IVector<T> => T.Norm(v);
        public static double Distance<T>(T v1, T v2) where T : IVector<T> => Distance(v1, v2);
        public static T Unitize<T>(T v) where T : IVector<T> => T.Unitize(v);
    }
}
