using Silk.NET.Input;
using Silk.NET.Maths;
using SourceRewrite.InputSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SourceRewrite.Windowing.Modules
{
    public class InputModule : IGameModule
    {
        public InputContext Context { get; private set; }

        public void OnStart()
        {
            var window = GameWindow.CurrentWindow?.GetSilkWindow();
            if (window != null)
            {
                Context = new InputContext(window.CreateInput());
            }
        }

        public void OnUpdate(double deltaTime)
        {
            Context?.InputUpdate();
        }

        public void OnRender(double deltaTime) { }

        public void OnShutdown() { }

        public void OnFramebufferResize(Vector2D<int> newSize) { }
    }
}
