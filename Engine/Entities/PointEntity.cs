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
        private Transform _transform = new Transform();
        public Transform Transform
        {
            get
            {
                if (Parent is PointEntity parent)
                {
                    return parent.Transform * LocalTransform;
                }
                return LocalTransform;
            }
        }

        public Transform LocalTransform
        {
            get => _transform;
            set => _transform = value;
        }

        // Name Constructor
        public PointEntity(string name) : base(name) { }

        // Nameless Constructor
        public PointEntity() : base() { }
    }
}
