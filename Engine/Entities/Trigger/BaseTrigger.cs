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
    public class BaseTrigger : BrushEntity
    {
        public Mesh CollisionMesh = new Mesh();

        public override void Start()
        {
            // Clear any previous physics bodies
            PhysicsDestroyObject();

            foreach (Mesh mesh in Brush)
            {
                // Add this mesh to collisionMesh
                CollisionMesh.Vertices = CollisionMesh.Vertices.Concat(mesh.Vertices).ToArray();
                CollisionMesh.Indices = CollisionMesh.Indices.Concat(mesh.Indices).ToArray();
            }

            // Physics Properties
            PhysicsBody.BodyType = BodyType.Static;
            PhysicsBody.CollisionMesh = CollisionMesh;
            PhysicsBody.CanCollide = false;
            
            // Init Physics
            PhysicsInitNormal();

            base.Start();
        }
    }
}
