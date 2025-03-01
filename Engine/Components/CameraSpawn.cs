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
        public override void Start()
        {
            // Set the active cameras position to the spawn position
            Camera.ActiveCamera.GameObject.Transform.Position = GameObject.Transform.Position;
        }
    }
}
