using System.Numerics;

namespace SourceRewrite.Entities
{
    public class Transform
    {
        public Vector3 Position { get; set; } = new Vector3(0, 0, 0);
        public Vector3 Scale { get; set; } = new Vector3(1, 1, 1);
        public Quaternion Rotation { get; set; } = Quaternion.Identity;

        public Vector3 Forward => Vector3.Transform(Vector3.UnitY, Rotation);
        public Vector3 Right => Vector3.Transform(Vector3.UnitX, Rotation);
        public Vector3 Up => Vector3.Transform(Vector3.UnitZ, Rotation);

        // OPERATORS
        public static Transform operator *(Transform a, Transform b)
        {
            return new Transform
            {
                Position = a.Position + Vector3.Transform(b.Position * a.Scale, a.Rotation),
                Scale = a.Scale * b.Scale,
                Rotation = Quaternion.Normalize(a.Rotation * b.Rotation)
            };
        }

        public static Transform operator *(Transform t, float scalar)
        {
            return new Transform
            {
                Position = t.Position * scalar,
                Scale = t.Scale * scalar,
                Rotation = Quaternion.Slerp(Quaternion.Identity, t.Rotation, scalar)
            };
        }

        public static Transform operator *(float scalar, Transform t) => t * scalar;
    }
}

