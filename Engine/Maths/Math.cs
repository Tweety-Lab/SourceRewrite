using System.Numerics;

namespace SourceRewrite
{
    public static partial class Math
    {
        /// <summary>
        /// Converts degrees to radians.
        /// </summary>
        public static float DegreesToRadians(float degrees)
        {
            return degrees * (MathF.PI / 180f);
        }

        /// <summary>
        /// Converts radians to degrees.
        /// </summary>
        public static float RadiansToDegrees(float radians)
        {
            return radians * (180f / MathF.PI);
        }

        /// <summary>
        /// Converts Euler degrees to Quaternion.
        /// </summary>
        public static Quaternion EulerToQuaternion(Vector3 euler)
        {
            // Convert Euler angles from degrees to radians
            float roll = DegreesToRadians(euler.X);  // x-axis rotation
            float pitch = DegreesToRadians(euler.Y); // y-axis rotation
            float yaw = DegreesToRadians(euler.Z);   // z-axis rotation

            // Compute the quaternion components
            float cy = MathF.Cos(yaw * 0.5f);
            float sy = MathF.Sin(yaw * 0.5f);
            float cr = MathF.Cos(roll * 0.5f);
            float sr = MathF.Sin(roll * 0.5f);
            float cp = MathF.Cos(pitch * 0.5f);
            float sp = MathF.Sin(pitch * 0.5f);

            // Calculate the quaternion components
            float w = cy * cr * cp + sy * sr * sp;
            float x = cy * sr * cp - sy * cr * sp;
            float y = cy * cr * sp + sy * sr * cp;
            float z = sy * cr * cp - cy * sr * sp;

            return new Quaternion(x, y, z, w);
        }

        /// <summary>
        /// Converts a Quaternion to Euler degrees.
        /// </summary>
        public static Vector3 QuaternionToEuler(Quaternion quat)
        {
            // Extract Euler angles (pitch, yaw, roll) in radians
            float pitch = MathF.Atan2(2.0f * (quat.W * quat.X + quat.Y * quat.Z), 1.0f - 2.0f * (quat.X * quat.X + quat.Y * quat.Y));
            float yaw = MathF.Asin(2.0f * (quat.W * quat.Y - quat.Z * quat.X));
            float roll = MathF.Atan2(2.0f * (quat.W * quat.Z + quat.X * quat.Y), 1.0f - 2.0f * (quat.Y * quat.Y + quat.Z * quat.Z));

            // Convert from radians to degrees
            pitch = RadiansToDegrees(pitch);
            yaw = RadiansToDegrees(yaw);
            roll = RadiansToDegrees(roll);

            return new Vector3(pitch, yaw, roll);
        }

        /// <summary>
        /// Returns the smaller of two values.
        /// </summary>
        public static float Min(float val1, float val2)
        {
            return val1 < val2 ? val1 : val2;
        }
    }
}