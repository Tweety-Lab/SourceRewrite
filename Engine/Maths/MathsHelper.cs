using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SourceRewrite.Maths
{
    public static class MathsHelper
    {
        public static float DegreesToRadians(float degrees)
        {
            return MathF.PI / 180f * degrees;
        }

        /// <summary>
        /// Convert Euler degrees to Quaternion.
        /// </summary>
        public static Quaternion EulerToQuaternion(Vector3 euler)
        {
            // Convert Euler angles from degrees to radians
            float roll = euler.X * (float)Math.PI / 180f;  // x-axis rotation
            float pitch = euler.Y * (float)Math.PI / 180f; // y-axis rotation
            float yaw = euler.Z * (float)Math.PI / 180f;   // z-axis rotation

            // Compute the quaternion components
            float cy = (float)Math.Cos(yaw * 0.5f);
            float sy = (float)Math.Sin(yaw * 0.5f);
            float cr = (float)Math.Cos(roll * 0.5f);
            float sr = (float)Math.Sin(roll * 0.5f);
            float cp = (float)Math.Cos(pitch * 0.5f);
            float sp = (float)Math.Sin(pitch * 0.5f);

            // Calculate the quaternion components
            float w = cy * cr * cp + sy * sr * sp;
            float x = cy * sr * cp - sy * cr * sp;
            float y = cy * cr * sp + sy * sr * cp;
            float z = sy * cr * cp - cy * sr * sp;

            return new Quaternion(x, y, z, w);
        }

        public static Vector3 QuaternionToEulerDegrees(Quaternion quat)
        {
            // Extract Euler angles (pitch, yaw, roll) in radians
            float pitch = MathF.Atan2(2.0f * (quat.W * quat.X + quat.Y * quat.Z), 1.0f - 2.0f * (quat.X * quat.X + quat.Y * quat.Y));
            float yaw = MathF.Asin(2.0f * (quat.W * quat.Y - quat.Z * quat.X));
            float roll = MathF.Atan2(2.0f * (quat.W * quat.Z + quat.X * quat.Y), 1.0f - 2.0f * (quat.Y * quat.Y + quat.Z * quat.Z));

            // Convert from radians to degrees
            pitch = pitch * (180.0f / MathF.PI);
            yaw = yaw * (180.0f / MathF.PI);
            roll = roll * (180.0f / MathF.PI);

            return new Vector3(pitch, yaw, roll);
        }
    }
}
