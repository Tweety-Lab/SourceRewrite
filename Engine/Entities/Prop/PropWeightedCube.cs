using SourceRewrite.Attributes;
using SourceRewrite.Entities.Meshes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SourceRewrite.Entities
{
    [Entity("prop_weighted_cube")]
    public class PropWeightedCube : PropPhysicsEntity
    {
        public override void Start()
        {
            MeshPath = "models/props/metal_box.mdl";
            base.Start();
        }
    }
}
