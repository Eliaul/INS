using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INS.Data
{
    internal class BinaryData : DataBase
    {
        private BinaryReader br = default!;

        public BinaryData(MotionState startState, double samplingRate, int maxRow, bool isCalLocalCartesianCoor = true, Func<double, bool>? zeroLambda = null)
            : base(startState, maxRow, isCalLocalCartesianCoor, zeroLambda)
        {
            IMUData.samplingRate = samplingRate;
        }

        public override void IMUDataCal(string imuPath)
        {
            br = new(new FileStream(imuPath, FileMode.Open));
            dataRow = 0;
            double[] datas = new double[7];
            Vector3d deltaAngle = new();
            Vector3d deltaVelocity = new();
            while (br.BaseStream.Position < br.BaseStream.Length)
            {
                for (int i = 0; i < 7; i++)
                {
                    datas[i] = br.ReadDouble();
                }
                deltaAngle.SetValue(datas[1], datas[2], datas[3]);
                deltaVelocity.SetValue(datas[4], datas[5], datas[6]);
                dataNow = new(datas[0], deltaAngle, deltaVelocity);
                if (datas[0] > startState.GPSSec)
                {
                    Update();
                }
                else
                { 
                    dataBack.SetValue(dataNow.GPSsec, dataNow.DeltaAngle, dataNow.DeltaVelocity);
                }
            }
            br.Close();
        }

        public override void ReferenceRead(string referencePath)
        {
            br = new(new FileStream(referencePath, FileMode.Open));
            dataRow = 0;
            while (br.BaseStream.Position < br.BaseStream.Length)
            {
                if ((result[dataRow, 0] = br.ReadDouble()) > startState.GPSSec)
                {
                    for (int i = 1; i < 10; i++)
                    {
                        result[dataRow, i] = br.ReadDouble();
                    }
                    if(isCalLocalCartesianCoor)
                    {
                        var NE = CalNE(Angle.Deg2Rad(result[dataRow, 1]), Angle.Deg2Rad(result[dataRow, 2]), result[dataRow, 3]);
                        result[dataRow, 10] = NE.Item1;
                        result[dataRow, 11] = NE.Item2;
                    }
                    dataRow++;
                }
            }
            br.Close();
        }

    }
}
