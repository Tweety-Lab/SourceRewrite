using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using BulletSharp;
using SourceRewrite.Entities;
using SourceRewrite.Windowing;
using SourceRewrite.Windowing.Modules;

namespace SourceRewrite.PhysicsSystem.Bullet
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

            dynamicsWorld.Gravity = new BulletSharp.Math.Vector3(0, -9.81f, 0); // Set gravity
        }

        public void Update()
        {
            // Update Bullet Physics
            dynamicsWorld.StepSimulation(1.0f / 22.2f, 10);

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

        public void InitPhysicsEntity(PointEntity entity, PhysicsBody physBody)
        {
            RigidBodyConstructionInfo bodyInfo;

            if (physBody.CollisionMesh != null)
            {
                // Create a collision shape from the mesh
                var meshShape = new ConvexHullShape(physBody.CollisionMesh.Vertices);

                // Create a rigid body construction info
                bodyInfo = new RigidBodyConstructionInfo(physBody.Mass, null, meshShape, BulletSharp.Math.Vector3.Zero);
            }
            else
            {
                // Create a collision shape from the bounding box
                var boxShape = new BoxShape(new BulletSharp.Math.Vector3(physBody.BoundingBox.X, physBody.BoundingBox.X, physBody.BoundingBox.Z));

                // Create a rigid body construction info
                bodyInfo = new RigidBodyConstructionInfo(1, null, boxShape, BulletSharp.Math.Vector3.Zero);
            }

            // Create a rigid body
            var body = new RigidBody(bodyInfo);

            // Set the rigid body's position and orientation
            var transform = BulletSharp.Math.Matrix.Translation(new BulletSharp.Math.Vector3(entity.Transform.Position.X, entity.Transform.Position.Y, entity.Transform.Position.Z)) * BulletSharp.Math.Matrix.RotationQuaternion(new BulletSharp.Math.Quaternion(entity.Transform.Rotation.X, entity.Transform.Rotation.Y, entity.Transform.Rotation.Z, entity.Transform.Rotation.W));
            body.WorldTransform = transform;

            // Set PhysicsBody properties
            BulletSharp.Math.Vector3 localInertia;
            bodyInfo.CollisionShape.CalculateLocalInertia(physBody.Mass, out localInertia);
            body.SetMassProps(physBody.Mass, localInertia);
            body.Friction = physBody.Friction;
            body.Restitution = physBody.Restitution;
            body.UpdateInertiaTensor();

            // Set Object Static
            if (physBody.IsStatic)
            {
                body.CollisionFlags |= CollisionFlags.StaticObject;
                body.SetMassProps(0, new BulletSharp.Math.Vector3(0, 0, 0));
                body.UpdateInertiaTensor();
            }


            // Add the body to the dynamics world
            dynamicsWorld.AddRigidBody(body);

            // Activate the body
            body.Activate();

            // Store the entity and its corresponding rigid body
            entities.Add(entity, body);
        }

        public void SetEntityAbsVelocity(PointEntity entity, Vector3 velocity)
        {
            if (entities.TryGetValue(entity, out RigidBody body))
            {
                body.Activate();
                body.LinearVelocity = new BulletSharp.Math.Vector3(velocity.X, velocity.Y, velocity.Z);
            }
        }

        public BaseEntity RayCast(PhysicsRay ray)
        {
            // Perform a raycast
            var rayFrom = new BulletSharp.Math.Vector3(ray.Origin.X, ray.Origin.Y, ray.Origin.Z);
            var rayTo = new BulletSharp.Math.Vector3(ray.Direction.X * 9999999f, ray.Direction.Y * 9999999f, ray.Direction.Z * 9999999f);
            var rayResult = new ClosestRayResultCallback(ref rayFrom, ref rayTo);
            dynamicsWorld.RayTest(rayFrom, rayTo, rayResult);

            if (rayResult.HasHit)
            {
                // Get the entity that was hit
                foreach (var entity in entities)
                {
                    if (entity.Value == rayResult.CollisionObject)
                    {
                        return entity.Key;
                    }
                }
            }

            return null; // No entity hit
        }
    }
}
