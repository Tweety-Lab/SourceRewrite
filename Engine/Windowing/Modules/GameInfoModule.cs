using FileFormats.KeyValues.GameInfo;
using Silk.NET.Input;
using Silk.NET.Maths;
using SourceRewrite.Files;
using SourceRewrite.InputSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SourceRewrite.Windowing.Modules
{
    public class GameInfoModule : IGameModule
    {
        public GameInfoFormat GameInfo { get; private set; }

        public void OnStart()
        {
            // Load GameInfo
            string gameInfoContent = File.ReadAllText("../../gameinfo.txt");
            GameInfo = new GameInfoFormat(gameInfoContent);

            // Set window title to game name as defined in GameInfo
            GameWindow.CurrentWindow.WindowTitle = GameInfo.GameName;
        }

        public void OnUpdate(double deltaTime) { }

        public void OnRender(double deltaTime) { }

        public void OnShutdown() { }

        public void OnFramebufferResize(Vector2D<int> newSize) { }
    }
}
