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
            RigidBodyConstructionInfo bodyInfo;

            if (entity.PhysicsBody.CollisionMesh != null)
            {
                // Create a collision shape from the mesh
                var meshShape = new ConvexHullShape(entity.PhysicsBody.CollisionMesh.Vertices);

                // Create a rigid body construction info
                bodyInfo = new RigidBodyConstructionInfo(entity.PhysicsBody.Mass, null, meshShape, BulletSharp.Math.Vector3.Zero);
            } else
            {
                // Create a collision shape from the bounding box
                var boxShape = new BoxShape(new BulletSharp.Math.Vector3(entity.PhysicsBody.BoundingBox.X, entity.PhysicsBody.BoundingBox.Y, entity.PhysicsBody.BoundingBox.Z));

                // Create a rigid body construction info
                bodyInfo = new RigidBodyConstructionInfo(1, null, boxShape, BulletSharp.Math.Vector3.Zero);
            }

            // Create a rigid body
            var body = new RigidBody(bodyInfo);

            // Set the rigid body's position and orientation
            var transform = BulletSharp.Math.Matrix.Translation(new BulletSharp.Math.Vector3(entity.Transform.Position.X, entity.Transform.Position.Y, entity.Transform.Position.Z)) * BulletSharp.Math.Matrix.RotationQuaternion(new BulletSharp.Math.Quaternion(entity.Transform.Rotation.X, entity.Transform.Rotation.Y, entity.Transform.Rotation.Z, entity.Transform.Rotation.W));
            body.WorldTransform = transform;

            // Set PhysicsBody properties
            body.SetMassProps(entity.PhysicsBody.Mass, new BulletSharp.Math.Vector3(0, 0, 0));
            body.Friction = entity.PhysicsBody.Friction;
            body.Restitution = entity.PhysicsBody.Restitution;
            body.UpdateInertiaTensor();

            // Set Object Static
            if (entity.PhysicsBody.IsStatic)
            {
                body.CollisionFlags |= CollisionFlags.StaticObject;
                body.SetMassProps(0, new BulletSharp.Math.Vector3(0, 0, 0));
                body.UpdateInertiaTensor();
            }


            // Add the body to the dynamics world
            dynamicsWorld.AddRigidBody(body);

            // Store the entity and its corresponding rigid body
            entities.Add(entity, body);
        }
    }
}
