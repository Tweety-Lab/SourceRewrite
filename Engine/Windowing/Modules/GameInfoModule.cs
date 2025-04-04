using FileFormats.KeyValues.GameInfo;
using Silk.NET.Maths;

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
