using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace INS.Data
{
    using Excel = Microsoft.Office.Interop.Excel;
    internal abstract class DataBase
    {
        public double[,] result = default!;
        public int maxRow = default!;
        public int dataRow = default!;
        public int dataCol = default!;
        protected MotionState mNow = default!;
        protected MotionState mBack = default!;
        protected MotionState mBBack = default!;
        protected readonly MotionState startState = default!;
        protected IMUData dataNow = default!;
        protected IMUData dataBack = default!;
        public bool isCalLocalCartesianCoor = true;
        protected Func<double, bool>? lambda;

        protected DataBase(MotionState startState, int maxRow, bool isCalLocalCartesianCoor, Func<double, bool>? lambda = null)
        {
            mBack = startState;
            mBBack = startState;
            this.startState = startState;
            mNow = new();
            dataNow = new();
            dataBack = new();
            dataRow = 0;
            this.isCalLocalCartesianCoor = isCalLocalCartesianCoor;
            this.maxRow = maxRow;
            dataCol = isCalLocalCartesianCoor ? 12 : 10;
            result = new double[maxRow, dataCol];
            this.lambda = lambda;
        }

        public abstract void IMUDataCal(string imuPath);
        public abstract void ReferenceRead(string referencePath);

        #region "IMU数据计算"
        protected void Update()
        {
            if (VelocityCorrect(dataNow.GPSsec, ref mNow, mBack))
            {
                mNow.GPSSec = dataNow.GPSsec;
                ResultArrayAssignment(ref result, dataRow, mNow);
                dataRow++;
                mBBack = mBack;
                mBack = mNow;
                dataBack.SetValue(dataNow.GPSsec, dataNow.DeltaAngle, dataNow.DeltaVelocity);
                return;
            }
            mNow = INSAlgorithm.Update(mBack, mBBack, dataNow, dataBack);
            ResultArrayAssignment(ref result, dataRow, mNow);
            dataRow++;
            mBBack = mBack;
            mBack = mNow;
            dataBack.SetValue(dataNow.GPSsec, dataNow.DeltaAngle, dataNow.DeltaVelocity);
        }

        private bool VelocityCorrect(double time, ref MotionState mNow, MotionState mBack)
        {
            if (lambda != null && lambda(time))
            {
                mNow = mBack.Clone();
                mNow.Velocity.SetValue(0, 0, 0);
                return true;
            }
            return false;
        }

        private void ResultArrayAssignment(ref double[,] result, int index, MotionState mNow)
        {
            EulerAngle eulerAngle = Quaternion.ToEulerAngle(mNow.Attitude);
            result[index, 0] = mNow.GPSSec;
            for (int j = 1; j < 4; j++)
            {
                result[index, j] = mNow.BLH[j - 1];
            }
            for (int j = 4; j < 7; j++)
            {
                result[index, j] = mNow.Velocity[j - 4];
            }
            for (int j = 7; j < 10; j++)
            {
                result[index, j] = eulerAngle[j - 7];
            }
            if(isCalLocalCartesianCoor)
            {
                var NE = CalNE(mNow.BLH.B, mNow.BLH.L, mNow.BLH.H);
                result[index, 10] = NE.Item1;
                result[index, 11] = NE.Item2;
            }
        }
        #endregion

        /// <summary>
        /// 辅助函数,计算参考结果的NE站心坐标
        /// </summary>
        /// <param name="b">弧度</param>
        /// <param name="l">弧度</param>
        /// <param name="h">弧度</param>
        /// <returns>N,E元组</returns>
        protected Tuple<double, double> CalNE(double b, double l, double h)
        {
            double a = Constant.GRS80.Semi_major;
            double e = Constant.GRS80.Eccentricity_1;
            double esinB2 = Math.Pow(Constant.GRS80.Eccentricity_1 * Math.Sin(b), 2);
            double R_M = a * (1 - e * e) / Math.Pow(1 - esinB2, 1.5);
            double R_N = a / Math.Sqrt(1 - esinB2);
            double N = (b - startState.BLH.B) * (R_M + h);
            double E = (l - startState.BLH.L) * (R_N + h) * Math.Cos(b);
            return new Tuple<double, double>(N, E);
        }

        public Excel.Worksheet CreatAndExportExcel(string sheetName)
        {
            string[] sheetHeader = !isCalLocalCartesianCoor ? new string[] { "周内秒(s)", "纬度(°)", "经度(°)", "椭球高(m)", "北向速度(m/s)", "东向速度(m/s)", "垂向速度(m/s)", "横滚角(°)", "俯仰角(°)", "航向角(°)" } : new string[] { "周内秒(s)", "纬度(°)", "经度(°)", "椭球高(m)", "北向速度(m/s)", "东向速度(m/s)", "垂向速度(m/s)", "横滚角(°)", "俯仰角(°)", "航向角(°)", "站心北向坐标(m)", "站心东向坐标(m)" };
            Excel.Application excel = new()
            {
                Visible = true,
            };
            Excel.Workbooks workbooks = excel.Workbooks;
            Excel.Workbook workbook = workbooks.Add(Excel.XlWBATemplate.xlWBATWorksheet);
            return ExportExcel.GetExcel(workbook, sheetName, sheetHeader, result, dataRow, result.GetLength(1));
        }

        public void AppendDataToExcel(Excel.Worksheet worksheet, Excel.Range startCell)
        {
            string[] sheetHeader = !isCalLocalCartesianCoor ? new string[] { "周内秒(s)", "纬度(°)", "经度(°)", "椭球高(m)", "北向速度(m/s)", "东向速度(m/s)", "垂向速度(m/s)", "横滚角(°)", "俯仰角(°)", "航向角(°)" } : new string[] { "周内秒(s)", "纬度(°)", "经度(°)", "椭球高(m)", "北向速度(m/s)", "东向速度(m/s)", "垂向速度(m/s)", "横滚角(°)", "俯仰角(°)", "航向角(°)", "站心北向坐标(m)", "站心东向坐标(m)" };
            ExportExcel.AddExcel(worksheet, startCell, sheetHeader, result, dataRow, result.GetLength(1));
        }

        public void CalAndExportExcel(string imuPath, string referencePath, string sheetName = "mySheet")
        {
            IMUDataCal(imuPath);
            var worksheet = CreatAndExportExcel(sheetName);
            ReferenceRead(referencePath);
            Excel.Range startCell = worksheet.Cells[2, result.GetLength(1) + 2];
            AppendDataToExcel(worksheet, startCell);
        }
    }
}
