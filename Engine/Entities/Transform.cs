using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SourceRewrite.Entities
{
    public class Transform
    {
        public Vector3 Position { get; set; } = new Vector3(0, 0, 0);
        public Vector3 Scale { get; set; } = new Vector3(1, 1, 1);
        public Quaternion Rotation { get; set; } = Quaternion.Identity;

        public Vector3 Forward => Vector3.Normalize(Vector3.Transform(-Vector3.UnitZ, Rotation));
        public Vector3 Right => Vector3.Normalize(Vector3.Transform(Vector3.UnitX, Rotation));
        public Vector3 Up => Vector3.Normalize(Vector3.Transform(Vector3.UnitY, Rotation));
    }
}

