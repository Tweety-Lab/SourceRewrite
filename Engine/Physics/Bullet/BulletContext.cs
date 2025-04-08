using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SourceRewrite.Physics.Bullet
{
    public class BulletContext : IPhysicsAPI
    {
        public void OnLoad()
        {
            // Initialize Bullet Physics
            Console.WriteLine("Bullet Physics Loaded");
        }

        public void Update()
        {
            // Update Bullet Physics
            Console.WriteLine("Bullet Physics Updated");
        }
    }
}
