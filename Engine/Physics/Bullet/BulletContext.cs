using System;
using System.Collections.Generic;
using System.Numerics;
using BulletSharp;
using SourceRewrite.Entities;
using SourceRewrite.Windowing.Modules;

namespace SourceRewrite.PhysicsSystem.Bullet
{
    public class BulletContext : IPhysicsAPI
    {
        CollisionConfiguration collisionConfiguration;
        CollisionDispatcher dispatcher;
        BroadphaseInterface broadphase;
        DiscreteDynamicsWorld dynamicsWorld;

        // Track bodies to entities
        Dictionary<BaseEntity, RigidBody> entityToBody = new Dictionary<BaseEntity, RigidBody>();
        Dictionary<RigidBody, BaseEntity> bodyToEntity = new Dictionary<RigidBody, BaseEntity>();

        public void OnLoad()
        {
            collisionConfiguration = new DefaultCollisionConfiguration();
            dispatcher = new CollisionDispatcher(collisionConfiguration);
            broadphase = new DbvtBroadphase();
            dynamicsWorld = new DiscreteDynamicsWorld(dispatcher, broadphase, null, collisionConfiguration);
            dynamicsWorld.Gravity = new BulletSharp.Math.Vector3(0, 0, -9.81f);
        }

        public void Update()
        {
            dynamicsWorld.StepSimulation(1.0f / 22.2f, 10);

            // Update all entities with their physics bodies
            foreach (var pair in entityToBody)
            {
                var entity = pair.Key;
                var body = pair.Value;

                if (entity is PointEntity pointEntity)
                {
                    var bulletMatrix = body.WorldTransform;

                    var numericsMatrix = new Matrix4x4(
                        bulletMatrix[0, 0], bulletMatrix[0, 1], bulletMatrix[0, 2], bulletMatrix[0, 3],
                        bulletMatrix[1, 0], bulletMatrix[1, 1], bulletMatrix[1, 2], bulletMatrix[1, 3],
                        bulletMatrix[2, 0], bulletMatrix[2, 1], bulletMatrix[2, 2], bulletMatrix[2, 3],
                        bulletMatrix[3, 0], bulletMatrix[3, 1], bulletMatrix[3, 2], bulletMatrix[3, 3]
                    );

                    pointEntity.Transform.Rotation = Quaternion.CreateFromRotationMatrix(numericsMatrix);
                    pointEntity.Transform.Position = new Vector3(
                        numericsMatrix.M41,
                        numericsMatrix.M42,
                        numericsMatrix.M43
                    );
                }
                // Brush entities don't get transformed as their geometry is world-aligned
            }
        }

        public void InitPhysicsEntity(BaseEntity entity, PhysicsBody physBody)
        {
            if (entityToBody.ContainsKey(entity))
            {
                Console.WriteLine("Entity already has a physics body");
                return;
            }

            RigidBody body = CreateRigidBody(entity, physBody);
            entityToBody[entity] = body;
            bodyToEntity[body] = entity;
        }

        private RigidBody CreateRigidBody(BaseEntity entity, PhysicsBody physBody)
        {
            RigidBodyConstructionInfo bodyInfo;
            CollisionShape shape;

            if (physBody.CollisionMesh != null)
            {
                shape = new ConvexHullShape(physBody.CollisionMesh.Vertices);
            }
            else
            {
                shape = new BoxShape(new BulletSharp.Math.Vector3(
                    physBody.BoundingBox.X,
                    physBody.BoundingBox.Y,
                    physBody.BoundingBox.Z));
            }

            bodyInfo = new RigidBodyConstructionInfo(physBody.Mass, null, shape, BulletSharp.Math.Vector3.Zero);
            var body = new RigidBody(bodyInfo);

            // Set initial transform
            if (entity is PointEntity pointEntity)
            {
                var rotation = BulletSharp.Math.Matrix.RotationQuaternion(new BulletSharp.Math.Quaternion(
                    pointEntity.Transform.Rotation.X,
                    pointEntity.Transform.Rotation.Y,
                    pointEntity.Transform.Rotation.Z,
                    pointEntity.Transform.Rotation.W));

                var translation = BulletSharp.Math.Matrix.Translation(new BulletSharp.Math.Vector3(
                    pointEntity.Transform.Position.X,
                    pointEntity.Transform.Position.Y,
                    pointEntity.Transform.Position.Z));

                body.WorldTransform = rotation * translation;
            }

            // Configure physics properties
            BulletSharp.Math.Vector3 localInertia;
            bodyInfo.CollisionShape.CalculateLocalInertia(physBody.Mass, out localInertia);
            body.SetMassProps(physBody.Mass, localInertia);
            body.Friction = physBody.Friction;
            body.Restitution = physBody.Restitution;
            body.UpdateInertiaTensor();

            if (physBody.CanCollide == false)
            {
                body.CollisionFlags |= CollisionFlags.NoContactResponse;
            }

            if (physBody.IsStatic)
            {
                body.CollisionFlags |= CollisionFlags.StaticObject;
                body.SetMassProps(0, new BulletSharp.Math.Vector3(0, 0, 0));
            }

            dynamicsWorld.AddRigidBody(body);
            body.Activate();

            return body;
        }

        public void DestroyPhysicsEntity(BaseEntity entity)
        {
            if (entityToBody.TryGetValue(entity, out var body))
            {
                dynamicsWorld.RemoveRigidBody(body);
                bodyToEntity.Remove(body);
                body.Dispose();
                entityToBody.Remove(entity);
            }
        }

        public void SetEntityAbsVelocity(BaseEntity entity, Vector3 velocity)
        {
            if (entityToBody.TryGetValue(entity, out var body))
            {
                body.Activate();
                body.LinearVelocity = new BulletSharp.Math.Vector3(velocity.X, velocity.Y, velocity.Z);
            }
        }

        public BaseEntity RayCast(PhysicsRay ray)
        {
            var rayFrom = new BulletSharp.Math.Vector3(ray.Origin.X, ray.Origin.Y, ray.Origin.Z);
            var rayTo = new BulletSharp.Math.Vector3(
                ray.Direction.X * 999999f,
                ray.Direction.Y * 999999f,
                ray.Direction.Z * 999999f);

            var rayResult = new ClosestRayResultCallback(ref rayFrom, ref rayTo);
            dynamicsWorld.RayTest(rayFrom, rayTo, rayResult);

            if (rayResult.HasHit && bodyToEntity.TryGetValue(rayResult.CollisionObject as RigidBody, out var entity))
            {
                return entity;
            }

            return null;
        }

        public void Dispose()
        {
            foreach (var body in entityToBody.Values)
            {
                dynamicsWorld.RemoveRigidBody(body);
                body.Dispose();
            }

            entityToBody.Clear();
            bodyToEntity.Clear();

            dynamicsWorld.Dispose();
            broadphase.Dispose();
            dispatcher.Dispose();
            collisionConfiguration.Dispose();
        }
    }
}