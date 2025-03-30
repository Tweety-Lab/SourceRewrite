using SourceRewrite.Attributes;
using SourceRewrite.Windowing;

namespace SourceRewrite.Entities.Console
{
    public static class ConVars
    {
        private static int _maxFps = 60;

        [ConVar("max_fps")]
        public static int MaxFPS
        {
            get => _maxFps;
            set
            {
                GameWindow.CurrentWindow.GetSilkWindow().UpdatesPerSecond = value;
            }
        }

        [ConVar("vsync")]
        public static bool VSync
        {
            get => GameWindow.CurrentWindow.GetSilkWindow().VSync;
            set
            {
                GameWindow.CurrentWindow.GetSilkWindow().VSync = value;
            }
        }
    }
}
