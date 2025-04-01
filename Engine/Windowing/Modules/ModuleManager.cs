using Silk.NET.Maths;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SourceRewrite.Windowing.Modules
{
    /// <summary>
    /// Handle Game Modules.
    /// </summary>
    public class ModuleManager
    {
        private readonly Dictionary<Type, IGameModule> _modules = new();

        public void RegisterModule<T>(T module) where T : IGameModule
        {
            _modules[typeof(T)] = module;
            module.OnStart();
        }

        public T? GetModule<T>() where T : class, IGameModule
        {
            if (_modules.TryGetValue(typeof(T), out var module))
            {
                return module as T;
            }
            return null;
        }

        public void UpdateModules(double deltaTime)
        {
            foreach (var module in _modules.Values)
                module.OnUpdate(deltaTime);
        }

        public void RenderModules(double deltaTime)
        {
            foreach (var module in _modules.Values)
                module.OnRender(deltaTime);
        }

        public void ResizeModules(Vector2D<int> newSize)
        {
            foreach (var module in _modules.Values)
                module.OnFramebufferResize(newSize);
        }

        public void ShutdownModules()
        {
            foreach (var module in _modules.Values)
                module.OnShutdown();
        }
    }
}
