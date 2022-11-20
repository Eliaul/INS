using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace INS.Data
{
    internal class ASCData : DataBase
    {
        private StreamReader sr = default!;

        public ASCData(MotionState startState, double samplingRate, int maxRow, bool isCalLocalCartesianCoor = true, Func<double, bool>? zeroLambda = null)
            :base(startState, maxRow, isCalLocalCartesianCoor, zeroLambda)
        {
            IMUData.samplingRate = samplingRate;
        }

        public override void IMUDataCal(string imuPath)
        {
            sr = new(imuPath);
            dataRow = 0;
            string? line;
            Vector3d deltaAngle = new();
            Vector3d deltaVelocity = new();
            while ((line = sr.ReadLine()) != null)
            {
                if (line[0] == '#')
                {
                    continue;
                }
                string[] lineData = line.Split(',', '*', ';');
                double time = Convert.ToDouble(lineData[2]);
                result[dataRow, 0] = time;
                deltaAngle.SetValue(-Convert.ToDouble(lineData[10]) * IMUData.gyro_scale, Convert.ToDouble(lineData[11]) * IMUData.gyro_scale, -Convert.ToDouble(lineData[9]) * IMUData.gyro_scale);
                deltaVelocity.SetValue(-Convert.ToDouble(lineData[7]) * IMUData.acc_scale, Convert.ToDouble(lineData[8]) * IMUData.acc_scale, -Convert.ToDouble(lineData[6]) * IMUData.acc_scale);
                dataNow = new(time, deltaAngle, deltaVelocity);
                if (time > startState.GPSSec)
                {
                    Update();
                }
                else
                {
                    dataBack.SetValue(time, deltaAngle, deltaVelocity);
                }
            }
            sr.Close();
        }

        public override void ReferenceRead(string referencePath)
        {
            sr = new(referencePath);
            dataRow = 0;
            string? line;
            while ((line = sr.ReadLine()) != null)
            {
                if (line.Length > 2 && Char.IsNumber(line[1]))
                {
                    string[] lineDataBack = new string[12];
                    do
                    {
                        line = line.TrimStart();
                        string[] lineData = Regex.Split(line, "\\s+", RegexOptions.IgnoreCase);
                        if (lineData[1] == lineDataBack[1])
                        {
                            continue;
                        }
                        if (Convert.ToDouble(lineData[1]) > startState.GPSSec)
                        {
                            for (int i = 0; i < 10; i++)
                            {
                                result[dataRow, i] = Convert.ToDouble(lineData[i + 1]);
                            }
                            if (isCalLocalCartesianCoor)
                            {
                                var ne = CalNE(Angle.Deg2Rad(result[dataRow, 1]), Angle.Deg2Rad(result[dataRow, 2]), result[dataRow, 3]);
                                result[dataRow, 10] = ne.Item1;
                                result[dataRow, 11] = ne.Item2;
                            }
                            dataRow++;
                        }
                        lineDataBack = lineData;
                    }
                    while ((line = sr.ReadLine()) != null);
                }
            }
            sr.Close();
        }

    }


}
