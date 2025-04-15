using SourceRewrite.AssetTypes;
using SourceRewrite.Attributes;
using SourceRewrite.PhysicsSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SourceRewrite.Entities
{
    [Entity("trigger_once")]
    public class BaseTrigger : BrushEntity
    {
        public override void Start()
        {
            Console.WriteLine("Starting Trigger");
            foreach (Mesh mesh in Brush)
            {
                PhysicsBody.IsStatic = true;
                PhysicsBody.CollisionMesh = mesh;
                PhysicsBody.CanCollide = false;
                PhysicsInitNormal();

                PhysicsBody.OnCollisionEnter += (entity) => { Console.WriteLine("Collision Detected"); };
            }

            base.Start();
        }
    }
}
