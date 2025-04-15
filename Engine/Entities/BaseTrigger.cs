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
        public Mesh CollisionMesh = new Mesh();

        public override void Start()
        {
            Console.WriteLine("Starting Trigger");
            foreach (Mesh mesh in Brush)
            {
                // Add this mesh to collisionMesh
                CollisionMesh.Vertices = CollisionMesh.Vertices.Concat(mesh.Vertices).ToArray();
                CollisionMesh.Indices = CollisionMesh.Indices.Concat(mesh.Indices).ToArray();
            }
            
            // Physics Properties
            PhysicsBody.IsStatic = true;
            PhysicsBody.CollisionMesh = CollisionMesh;
            PhysicsBody.CanCollide = false;
            
            // Init Physics
            PhysicsInitNormal();

            // Register Physics Events
            PhysicsBody.OnCollisionStart += (entity) => { Console.WriteLine("Collision Detected"); };
            PhysicsBody.OnCollisionEnd += (entity) => { Console.WriteLine("Collision Ended"); };

            base.Start();
        }
    }
}
