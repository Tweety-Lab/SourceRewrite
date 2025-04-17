using SourceRewrite.Rendering.OpenGL;
using SourceRewrite.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SourceRewrite.PhysicsSystem.Bullet;
using SourceRewrite.TimeSystem;
using System.Numerics;
using SourceRewrite.Entities;

namespace SourceRewrite.PhysicsSystem
{
    public class PhysicsContext
    {
        /// <summary>
        /// The physics API. (e.g., Bullet, Jolt, etc).
        /// </summary>
        public PhysicsAPI API { get; private set; }

        // Mapping of physics to physics contexts
        private Dictionary<PhysicsAPI, Type> PhysicsMap = new Dictionary<PhysicsAPI, Type>
        {
            { PhysicsAPI.Bullet, typeof(BulletContext) }
        };

        public readonly IPhysicsAPI APIInterface; // Use an interface for better abstraction
        public PhysicsContext(PhysicsAPI chosenPhysics)
        {
            API = chosenPhysics; // Pass chosen renderer to our API variable

            // Get the Chosen Renderer Context
            PhysicsMap.TryGetValue(chosenPhysics, out Type physicsType);

            // Create the Physics Context
            if (physicsType != null)
            {
                APIInterface = (IPhysicsAPI)Activator.CreateInstance(physicsType);
            }
            else
            {
                Console.WriteLine("Physics API type not found.");
            }
        }

        public void OnLoad()
        {
            APIInterface.OnLoad();
        }

        public void Update()
        {
            APIInterface.Update();
        }
    }

    public interface IPhysicsAPI
    {
        /// <summary>
        /// Called once on physics load.
        /// </summary>
        void OnLoad();

        /// <summary>
        /// Called once per physics tick.
        /// </summary>
        void Update();

        /// <summary>
        /// Allows an Entity to have Physics.
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <param name="kinematic"></param>
        void InitPhysicsEntity(BaseEntity physEntity, PhysicsBody body);

        /// <summary>
        /// Destroy a Physics for an Entity.
        /// </summary>
        /// <param name="physEntity"></param>
        void DestroyPhysicsEntity(BaseEntity physEntity);

        /// <summary>
        /// Set the absolute velocity of an Entity.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="velocity"></param>
        void SetEntityAbsVelocity(BaseEntity entity, Vector3 velocity);

        /// <summary>
        /// Set the transform of an Entity.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="transform"></param>
        /// <param name="isTeleport"></param>
        void SetEntityTransform(BaseEntity entity, Transform transform, bool isTeleport = false);

        /// <summary>
        /// Raycast from the origin to the direction.
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="direction"></param>
        /// <returns>Hit Entity</returns>
        BaseEntity RayCast(PhysicsRay ray);
    }

    /// <summary>
    /// Supported Physics APIs.
    /// </summary>
    public enum PhysicsAPI
    {
        Bullet
    }
}
