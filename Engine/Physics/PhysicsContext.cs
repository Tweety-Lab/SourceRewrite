using SourceRewrite.Rendering.OpenGL;
using SourceRewrite.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SourceRewrite.Physics.Bullet;
using SourceRewrite.TimeSystem;

namespace SourceRewrite.Physics
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

        // Physics timing variables
        private double _physicsAccumulator = 0.0;
        private double _physicsTimeStep = 1.0 / 60.0; // Default to 60 Tick-Rate


        /// <summary>
        /// The physics tick rate in updates per second.
        /// </summary>
        public double PhysicsTickRate
        {
            get => 1.0 / _physicsTimeStep;
            set => _physicsTimeStep = 1.0 / Math.Max(value, 1.0); // Ensure at least 1Hz
        }

        private readonly IPhysicsAPI _apiInterface; // Use an interface for better abstraction
        public PhysicsContext(PhysicsAPI chosenPhysics)
        {
            API = chosenPhysics; // Pass chosen renderer to our API variable

            // Get the Chosen Renderer Context
            PhysicsMap.TryGetValue(chosenPhysics, out Type physicsType);

            // Create the Physics Context
            if (physicsType != null)
            {
                _apiInterface = (IPhysicsAPI)Activator.CreateInstance(physicsType);
            }
            else
            {
                Console.WriteLine("Physics API type not found.");
            }
        }

        public void OnLoad()
        {
            _apiInterface.OnLoad();
        }

        public void Update()
        {
            // Get delta time
            double deltaTime = Time.DeltaTime;

            // Add to accumulator
            _physicsAccumulator += deltaTime;

            // Fixed timestep physics updates
            while (_physicsAccumulator >= _physicsTimeStep)
            {
                _apiInterface.Update();
                _physicsAccumulator -= _physicsTimeStep;
            }
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
    }

    /// <summary>
    /// Supported Physics APIs.
    /// </summary>
    public enum PhysicsAPI
    {
        Bullet
    }
}
