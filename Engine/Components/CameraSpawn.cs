using SourceRewrite.Maps;
using SourceRewrite.Maths;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SourceRewrite.Components
{
    public class CameraSpawn : GameComponent
    {
        [MapProperty("angles")]
        public Vector3 Angles;

        public override void Start()
        {
            // Set the active cameras position and rotation
            Camera.ActiveCamera.GameObject.Transform.Position = GameObject.Transform.Position;
            Camera.ActiveCamera.GameObject.Transform.Rotation = MathsHelper.EulerToQuaternion(Angles);
        }
    }
}
