using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using BulletSharp;

namespace SourceRewrite.Physics.Bullet
{
    public class BulletContext : IPhysicsAPI
    {
        CollisionConfiguration collisionConfiguration;
        CollisionDispatcher dispatcher;
        BroadphaseInterface broadphase;
        DiscreteDynamicsWorld dynamicsWorld;


        public void OnLoad()
        {
            // Initialize Bullet Physics
            collisionConfiguration = new DefaultCollisionConfiguration();
            dispatcher = new CollisionDispatcher(collisionConfiguration);

            broadphase = new DbvtBroadphase();

            dynamicsWorld = new DiscreteDynamicsWorld(dispatcher, broadphase, null, collisionConfiguration);
        }

        public void Update()
        {
            // Update Bullet Physics
            dynamicsWorld.StepSimulation(1.0f / 60.0f, 10);
        }

        public void BBoxToCollideable(Vector3 min, Vector3 max, bool kinematic)
        {
            // Create a collision shape from the bounding box
            var boxShape = new BoxShape(new BulletSharp.Math.Vector3(max.X - min.X, max.Y - min.Y, max.Z - min.Z) / 2);

            // Create a rigid body construction info
            var bodyInfo = new RigidBodyConstructionInfo(1, null, boxShape, BulletSharp.Math.Vector3.Zero);

            // Create a rigid body
            var body = new RigidBody(bodyInfo);

            // Add the body to the dynamics world
            dynamicsWorld.AddRigidBody(body);
        }
    }
}
