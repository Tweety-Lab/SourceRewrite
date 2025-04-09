using SourceRewrite.PhysicsSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SourceRewrite.Windowing.Modules
{
    public class PhysicsModule : IGameModule
    {
        public PhysicsContext Context { get; private set; }

        public void OnStart()
        {
            Context = new PhysicsContext(PhysicsAPI.Bullet);
            Context.OnLoad();
        }

        
        public void OnUpdate(double deltaTime)
        {
            Context.Update();
        }

        public void OnRender(double deltaTime) { }

        public void OnShutdown() { }

        public void OnFramebufferResize(Silk.NET.Maths.Vector2D<int> newSize) { }
    }
}
