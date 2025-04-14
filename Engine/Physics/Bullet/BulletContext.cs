using System;
using System.Collections.Generic;
using System.Linq;
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

        // Track multiple bodies per entity
        Dictionary<BaseEntity, List<RigidBody>> entityBodies = new Dictionary<BaseEntity, List<RigidBody>>();
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
            foreach (var entityPair in entityBodies)
            {
                var entity = entityPair.Key;
                var bodies = entityPair.Value;

                if (bodies.Count == 0) continue;

                // For point entities, use the first body's transform
                if (entity is PointEntity pointEntity)
                {
                    var primaryBody = bodies[0];
                    var bulletMatrix = primaryBody.WorldTransform;

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

                    // Update any additional bodies to match the entity's transform
                    for (int i = 1; i < bodies.Count; i++)
                    {
                        var body = bodies[i];
                        body.WorldTransform = bulletMatrix;
                    }
                }
                // Brush entities don't get transformed as their geometry is world-aligned
            }
        }

        public void InitPhysicsEntity(BaseEntity entity, PhysicsBody physBody)
        {
            if (!entityBodies.ContainsKey(entity))
            {
                entityBodies[entity] = new List<RigidBody>();
            }

            RigidBody body = CreateRigidBody(entity, physBody);
            entityBodies[entity].Add(body);
            bodyToEntity[body] = entity;
        }

        public void InitPhysicsEntity(BaseEntity entity, IEnumerable<PhysicsBody> physBodies)
        {
            if (!entityBodies.ContainsKey(entity))
            {
                entityBodies[entity] = new List<RigidBody>();
            }

            foreach (var physBody in physBodies)
            {
                RigidBody body = CreateRigidBody(entity, physBody);
                entityBodies[entity].Add(body);
                bodyToEntity[body] = entity;
            }
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
            if (entityBodies.TryGetValue(entity, out var bodies))
            {
                foreach (var body in bodies)
                {
                    dynamicsWorld.RemoveRigidBody(body);
                    bodyToEntity.Remove(body);
                    body.Dispose();
                }
                entityBodies.Remove(entity);
            }
        }

        public void SetEntityAbsVelocity(BaseEntity entity, Vector3 velocity)
        {
            if (entityBodies.TryGetValue(entity, out var bodies))
            {
                foreach (var body in bodies)
                {
                    body.Activate();
                    body.LinearVelocity = new BulletSharp.Math.Vector3(velocity.X, velocity.Y, velocity.Z);
                }
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
            foreach (var bodyList in entityBodies.Values)
            {
                foreach (var body in bodyList)
                {
                    dynamicsWorld.RemoveRigidBody(body);
                    body.Dispose();
                }
            }

            entityBodies.Clear();
            bodyToEntity.Clear();

            dynamicsWorld.Dispose();
            broadphase.Dispose();
            dispatcher.Dispose();
            collisionConfiguration.Dispose();
        }
    }
}