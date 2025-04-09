using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SourceRewrite.PhysicsSystem
{
    public class PhysicsRay
    {
        public Vector3 Origin { get; set; }
        public Vector3 Direction { get; set; }

        public PhysicsRay() { }

        public PhysicsRay(Vector3 origin, Vector3 direction)
        {
            Origin = origin;
            Direction = direction;
        }

    }
}
