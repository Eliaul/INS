

namespace INS
{
    internal static class INSAlgorithm
    {
        public static MotionState Update(MotionState mBack, MotionState mBBack, IMUData dataNow, IMUData dataBack)
        {
            deltaT = IMUData.samplingRate;
            Quaternion attitude = AttitudeUpdate(mBack, dataBack, dataNow);
            Vector3d velocity = VelocityUpdate(mBBack, mBack, dataBack, dataNow);
            BLHCoordinate position = PositionUpdate(mBack, mBBack, velocity);
            return new MotionState(attitude, position, velocity, dataNow.GPSsec);
        }

        private static double deltaT = 0;
        /// <summary>
        /// 姿态更新算法
        /// </summary>
        /// <param name="mBack">前一时刻的运动状态</param>
        /// <param name="thetaBack">前一时刻的角增量</param>
        /// <param name="thetaNow">当前时刻的角增量</param>
        /// <returns>当前时刻的姿态(q_b^n)</returns>
        private static Quaternion AttitudeUpdate(MotionState mBack, IMUData imuBack, IMUData imuNow)
        {
            //更新b系
            Vector3d phi_k = imuNow.DeltaAngle + Vector3d.CrossProduct(imuBack.DeltaAngle, imuNow.DeltaAngle) / 12;
            Quaternion q1 = Quaternion.FromRotationVector(phi_k);
             
            //更新n系. 
            Vector3d zeta_k = (mBack.Omega_ie + mBack.Omega_en) * deltaT;
            Quaternion q2 = Quaternion.FromRotationVector(-zeta_k);

            //计算当前姿态的四元数
            Quaternion q = q2 * mBack.Attitude * q1;
            return Quaternion.Normalize(q);
        }

        /// <summary>
        /// 速度更新算法
        /// </summary>
        /// <param name="mBBack">前两个时刻的运动状态</param>
        /// <param name="mBack">前一时刻的运动状态</param>
        /// <param name="imuDataBack">前一时刻的陀螺仪输出</param>
        /// <param name="imuDataNow">当前时刻的陀螺仪</param>
        /// <returns>当前时刻的速度(v_k^n)</returns>
        private static Vector3d VelocityUpdate(MotionState mBBack, MotionState mBack, IMUData imuDataBack, IMUData imuDataNow)
        {
            Vector3d v1 = imuDataNow.DeltaVelocity + Vector3d.CrossProduct(imuDataNow.DeltaAngle, imuDataNow.DeltaVelocity) / 2
                + (Vector3d.CrossProduct(imuDataBack.DeltaAngle, imuDataNow.DeltaVelocity) + Vector3d.CrossProduct(imuDataBack.DeltaVelocity, imuDataNow.DeltaAngle)) / 12;
            Vector3d Omega_ieMid = 1.5 * mBack.Omega_ie - 0.5 * mBBack.Omega_ie;
            Vector3d Omega_enMid = 1.5 * mBack.Omega_en - 0.5 * mBBack.Omega_en;
            Vector3d gMid = mBack.Gravity * 1.5 - mBBack.Gravity * 0.5;
            Vector3d zeta = (Omega_ieMid + Omega_enMid) * deltaT;
            Vector3d v2 = (Matrix3d.Identity() - (zeta.SkewSymmetricMatrix() / 2)) * Quaternion.ToRotationMatrix(mBack.Attitude) * v1;
            Vector3d tempBack = Vector3d.CrossProduct(2 * mBack.Omega_ie + mBack.Omega_en, mBack.Velocity);
            Vector3d tempBBack = Vector3d.CrossProduct(2 * mBBack.Omega_ie + mBBack.Omega_en, mBBack.Velocity);
            Vector3d tempMid = 1.5 * tempBack - 0.5 * tempBBack;
            Vector3d v3 = (gMid - tempMid) * deltaT;
            Vector3d temp = v2 + v3;
            return mBack.Velocity + v2 + v3;
        }

        /// <summary>
        /// 位置更新算法
        /// </summary>
        /// <param name="mBack">前一时刻的运动状态</param>
        /// <param name="mBBack">前两个时刻的运动状态</param>
        /// <param name="vNow">当前时刻的速度</param>
        /// <returns>当前时刻的位置(BLH)</returns>
        private static BLHCoordinate PositionUpdate(MotionState mBack, MotionState mBBack, Vector3d vNow)
        {
            double R_NMid = mBack.R_N * 1.5 - mBBack.R_N * 0.5;
            double H = mBack.BLH.H - (mBack.Velocity[2] + vNow[2]) * deltaT * 0.5;
            double HAve = (H + mBack.BLH.H) / 2;
            double B = mBack.BLH.B + (mBack.Velocity[0] + vNow[0]) * deltaT * 0.5 / (mBack.R_M + HAve);
            double BAve = (B + mBack.BLH.B) / 2;
            double L = mBack.BLH.L + (mBack.Velocity[1] + vNow[1]) * deltaT * 0.5 / ((R_NMid + HAve) * Math.Cos(BAve));
            return new BLHCoordinate(B, L, H);
        }

    }
}
