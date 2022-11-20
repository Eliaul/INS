// See https://aka.ms/new-console-template for more information

using INS;
using INS.Data;
using System.Data;
using System.Diagnostics;
using System.Text.Json;
using Excel = Microsoft.Office.Interop.Excel;

//{
//    Stopwatch sw = new();
//    sw.Start();
//    Quaternion q0 = EulerAngle.ToQuaternion(new EulerAngle(Angle.Deg2Rad(0.0107951084511778), Angle.Deg2Rad(-2.14251290749072), Angle.Deg2Rad(-75.7498049314083)));
//    BLHCoordinate b0 = new(Angle.Deg2Rad(23.1373950708), Angle.Deg2Rad(113.3713651222), 2.175);
//    Vector3d v0 = new(0d, 0d, 0d);
//    MotionState startState = new(q0, b0, v0, 91620d);
//    DataBase data = new BinaryData(startState, 0.005, 910000, isCalLocalCartesianCoor: true);
//    data.CalAndExportExcel("IMU.bin", "PureINS.bin", "示例数据");
//    sw.Stop();
//    Console.WriteLine("示例数据计算完成!,用时" + sw.Elapsed.TotalSeconds + "s\r\n");
//}


//{
//    Stopwatch sw = new();
//    sw.Start();
//    Quaternion q0 = EulerAngle.ToQuaternion(new EulerAngle(Angle.Deg2Rad(0.12238447), Angle.Deg2Rad(-0.10672794), Angle.Deg2Rad(356.76535600)));
//    BLHCoordinate b0 = new(Angle.Deg2Rad(30.5279246826), Angle.Deg2Rad(114.3556809769), 22.183);
//    Vector3d v0 = new(-0.002, 0.0, -0.001);
//    MotionState startState = new(q0, b0, v0, 450501.02);
//    Func<double, bool> zeroVelCorrect = time => time < 450799.950 || (time > 451033.64 && time < 451096.48) || (time > 451342.26 && time < 451405.06) || (time > 451487.10 && time < 451530.34)
//                || (time > 451652.58 && time < 451710.89) || (time > 451952.42 && time < 452016.83) || (time > 452245.30 && time < 452313.44) || time > 452559.93;
//    DataBase data = new ASCData(startState, 0.01, 300000, zeroLambda: zeroVelCorrect);
//    data.CalAndExportExcel("ReceivedTofile-COM4-2022_10_21_13-09-37.ASC", "gins.nav", "实验数据");
//    sw.Stop();
//    Console.WriteLine("实验数据计算完成!,用时" + sw.Elapsed.TotalSeconds + "s\r\n");
//}

{
    Stopwatch sw = new();
    sw.Start();
    Quaternion q0 = EulerAngle.ToQuaternion(new EulerAngle(Angle.Deg2Rad(0.11279989), Angle.Deg2Rad(-0.11981501), Angle.Deg2Rad(356.73460249)));
    BLHCoordinate b0 = new(Angle.Deg2Rad(30.5279246711), Angle.Deg2Rad(114.3556809712), 22.195);
    Vector3d v0 = new(-0.000, 0.0, -0.000);
    MotionState startState = new(q0, b0, v0, 450501.02);
    Func<double, bool> zeroVelCorrect = time => time < 450799.950 || (time > 451033.64 && time < 451096.48) || (time > 451342.26 && time < 451405.06) || (time > 451487.10 && time < 451530.34)
                || (time > 451652.58 && time < 451710.89) || (time > 451952.42 && time < 452016.83) || (time > 452245.30 && time < 452313.44) || time > 452559.93;
    DataBase data = new ASCData(startState, 0.01, 300000, zeroLambda: null);
    data.CalAndExportExcel("ReceivedTofile-COM4-2022_10_21_13-09-37.ASC", "gins.nav", "实验数据");
    sw.Stop();
    Console.WriteLine("实验数据计算完成!,用时" + sw.Elapsed.TotalSeconds + "s\r\n");
}

//{
//    Quaternion q0 = new(0, 0, 0, 0);
//    BLHCoordinate b0 = new(Angle.Deg2Rad(30.5280400639), Angle.Deg2Rad(114.3556924677), 22.248);
//    Vector3d v0 = new(-0.001, 0.0, -0.002);
//    MotionState startState = new(q0, b0, v0, 443364.015);
//    DataBase data = new ASCData(startState, 0.005, 500000);
//    data.ReferenceRead("品gins.nav");
//    data.CreatAndExportExcel("品");
//}