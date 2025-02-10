using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SourceRewrite.Components
{
    public class Transform : GameComponent
    {
        public Vector3 Position { get; set; } = new Vector3(0, 0, 0);

        public float Scale { get; set; } = 1f;

        public Quaternion Rotation { get; set; } = Quaternion.Identity;

        //Note: The order here does matter.
        public Matrix4x4 ViewMatrix => Matrix4x4.Identity * Matrix4x4.CreateFromQuaternion(Rotation) * Matrix4x4.CreateScale(Scale) * Matrix4x4.CreateTranslation(Position);

        /// <summary>
        /// Rotate the transform from Degrees.
        /// </summary>
        public void Rotate(float x, float y, float z)
        {
            // Convert Euler angles from degrees to radians (if needed)
            float pitchRad = x * MathF.PI / 180f;
            float yawRad = y * MathF.PI / 180f;
            float rollRad = z * MathF.PI / 180f;

            // Compute the sine and cosine of half angles
            float cy = MathF.Cos(yawRad * 0.5f);
            float sy = MathF.Sin(yawRad * 0.5f);
            float cp = MathF.Cos(pitchRad * 0.5f);
            float sp = MathF.Sin(pitchRad * 0.5f);
            float cr = MathF.Cos(rollRad * 0.5f);
            float sr = MathF.Sin(rollRad * 0.5f);

            // Compute the quaternion
            float qw = cy * cp * cr + sy * sp * sr;
            float qx = sy * cp * cr - cy * sp * sr;
            float qy = cy * sp * cr + sy * cp * sr;
            float qz = cy * cp * sr - sy * sp * cr;

            Rotation = new Quaternion(qx, qy, qz, qw);
        }
    }
}
