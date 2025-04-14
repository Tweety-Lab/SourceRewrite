using SourceRewrite.PhysicsSystem;
using SourceRewrite.Windowing;
using SourceRewrite.Windowing.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SourceRewrite.Entities
{
    /// <summary>
    /// An Entity that is a point in space.
    /// </summary>
    public class PointEntity : BaseEntity
    {
        // Entity Transform
        public Transform Transform { get; set; } = new Transform();

        // Name Constructor
        public PointEntity(string name) : base(name) { }

        // Nameless Constructor
        public PointEntity() : base() { }
    }
}
