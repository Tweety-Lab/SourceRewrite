using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SourceRewrite.Entities
{
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
