using Silk.NET.Maths;
using SourceRewrite.Entities;
using SourceRewrite.Files;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SourceRewrite.Windowing.Modules
{
    public class ConsoleModule : IGameModule
    {

        public void OnStart()
        {
            // Load Console as Global Entity
            EntityManager.AddGlobalEntity(new DeveloperConsoleCanvas());

            // Load config.cfg
            DeveloperConsole.LoadConfig(FileSystem.GamePath.BasePath + "cfg/config.cfg");

            // Load autoexec.cfg
            DeveloperConsole.LoadConfig(FileSystem.GamePath.BasePath + "cfg/autoexec.cfg");
        }

        public void OnUpdate(double deltaTime) { }

        public void OnRender(double deltaTime) { }

        public void OnShutdown()
        {
            // Write config.cfg
            DeveloperConsole.WriteConfig(FileSystem.GamePath.BasePath + "cfg/config.cfg");
        }

        public void OnFramebufferResize(Vector2D<int> newSize) { }
    }
}
