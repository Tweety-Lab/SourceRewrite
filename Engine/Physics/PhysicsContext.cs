using SourceRewrite.Rendering.OpenGL;
using SourceRewrite.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SourceRewrite.Physics.Bullet;

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

        private readonly IPhysicsAPI _apiInterface; // Use an interface for better abstraction
        public PhysicsContext(PhysicsAPI physicsAPI)
        {
            API = physicsAPI; // Pass chosen renderer to our API variable

            // Get the Chosen Renderer Context
            PhysicsMap.TryGetValue(physicsAPI, out Type physicsType);

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

        }

        public void Update()
        {

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
