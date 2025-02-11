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

        /// <summary>
        /// Forward Vector.
        /// </summary>
        public Vector3 Forward => Vector3.Transform(-Vector3.UnitZ, Rotation);

        /// <summary>
        /// Right Vector.
        /// </summary>
        public Vector3 Right => Vector3.Transform(Vector3.UnitX, Rotation);

        /// <summary>
        /// Up Vector.
        /// </summary>
        public Vector3 Up => Vector3.Transform(Vector3.UnitY, Rotation);

        //Note: The order here does matter.
        public Matrix4x4 ViewMatrix => Matrix4x4.Identity * Matrix4x4.CreateFromQuaternion(Rotation) * Matrix4x4.CreateScale(Scale) * Matrix4x4.CreateTranslation(Position);

        /// <summary>
        /// Rotate to the input Degrees.
        /// </summary>
        public void RotateTo(float x, float y, float z)
        {
            // Convert Euler angles from degrees to radians
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

        /// <summary>
        /// Rotate by the input Degrees. (Adds to rotation, doesn't set it)
        /// </summary>
        public void RotateBy(float x, float y, float z)
        {
            // Convert Euler angles from degrees to radians
            float pitchRad = x * MathF.PI / 180f;
            float yawRad = y * MathF.PI / 180f;
            float rollRad = z * MathF.PI / 180f;

            // Create rotation quaternions for each axis
            Quaternion rotX = Quaternion.CreateFromAxisAngle(Vector3.UnitX, pitchRad);
            Quaternion rotY = Quaternion.CreateFromAxisAngle(Vector3.UnitY, yawRad);
            Quaternion rotZ = Quaternion.CreateFromAxisAngle(Vector3.UnitZ, rollRad);

            // Combine the rotations (order matters: Z * Y * X is common)
            Quaternion deltaRotation = rotZ * rotY * rotX;

            // Apply the new rotation to the current rotation
            Rotation = Rotation * deltaRotation;

            // Normalize to prevent accumulation of floating-point errors
            Rotation = Quaternion.Normalize(Rotation);
        }
    }
}
