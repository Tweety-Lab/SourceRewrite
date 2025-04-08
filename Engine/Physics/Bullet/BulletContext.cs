using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using BulletSharp;
using SourceRewrite.Entities;

namespace SourceRewrite.Physics.Bullet
{
    public class BulletContext : IPhysicsAPI
    {
        CollisionConfiguration collisionConfiguration;
        CollisionDispatcher dispatcher;
        BroadphaseInterface broadphase;
        DiscreteDynamicsWorld dynamicsWorld;

        Dictionary<PointEntity, RigidBody> entities = new Dictionary<PointEntity, RigidBody>();


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

            // Update the entities
            foreach (var entity in entities)
            {
                var pointEntity = entity.Key;
                var body = entity.Value;

                var bulletMatrix = body.WorldTransform;
                var numericsMatrix = new Matrix4x4(
                    bulletMatrix[0, 0], bulletMatrix[0, 1], bulletMatrix[0, 2], bulletMatrix[0, 3],
                    bulletMatrix[1, 0], bulletMatrix[1, 1], bulletMatrix[1, 2], bulletMatrix[1, 3],
                    bulletMatrix[2, 0], bulletMatrix[2, 1], bulletMatrix[2, 2], bulletMatrix[2, 3],
                    bulletMatrix[3, 0], bulletMatrix[3, 1], bulletMatrix[3, 2], bulletMatrix[3, 3]
                );

                // Set Transforms
                pointEntity.Transform.Rotation = Quaternion.CreateFromRotationMatrix(numericsMatrix);
                pointEntity.Transform.Position = new Vector3(
                    numericsMatrix.M41,
                    numericsMatrix.M42,
                    numericsMatrix.M43
                );
            }
        }

        public void InitPhysicsEntity(PointEntity entity)
        {
            // Create a collision shape from the bounding box
            var boxShape = new BoxShape(new BulletSharp.Math.Vector3(128, 128, 128));

            // Create a rigid body construction info
            var bodyInfo = new RigidBodyConstructionInfo(1, null, boxShape, BulletSharp.Math.Vector3.Zero);

            // Create a rigid body
            var body = new RigidBody(bodyInfo);

            // Add the body to the dynamics world
            dynamicsWorld.AddRigidBody(body);

            // Store the entity and its corresponding rigid body
            entities.Add(entity, body);
        }
    }
}
