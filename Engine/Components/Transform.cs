using System.Numerics;

namespace SourceRewrite.Components
{
    /// <summary>
    /// GameObject Transform Component, manages position, rotation, scale, etc.
    /// </summary>
    public class Transform : GameComponent
    {
        public Vector3 Position { get; set; } = new Vector3(0, 0, 0);

        public Vector3 Scale { get; set; } = new Vector3(1, 1, 1);

        public Quaternion Rotation { get; set; } = Quaternion.Identity;

        /// <summary>
        /// Forward Vector.
        /// </summary>
        public Vector3 Forward => Vector3.Normalize(Vector3.Transform(-Vector3.UnitZ, Rotation));

        /// <summary>
        /// Right Vector.
        /// </summary>
        public Vector3 Right => Vector3.Normalize(Vector3.Transform(Vector3.UnitX, Rotation));

        /// <summary>
        /// Up Vector.
        /// </summary>
        public Vector3 Up => Vector3.Normalize(Vector3.Transform(Vector3.UnitY, Rotation));

        //Note: The order here does matter.
        public Matrix4x4 ViewMatrix => Matrix4x4.CreateTranslation(Position) * Matrix4x4.CreateFromQuaternion(Rotation) * Matrix4x4.CreateScale(Scale);
    }
}
