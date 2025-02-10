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
