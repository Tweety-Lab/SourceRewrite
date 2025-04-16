using SourceRewrite.Attributes;
using SourceRewrite.Windowing;

namespace SourceRewrite.Entities
{
    public static class ConVars
    {
        private static int _maxFps = 60;

        // Max FPS
        [ConVar("max_fps")]
        public static int MaxFPS
        {
            get => _maxFps;
            set
            {
                GameWindow.CurrentWindow.GetSilkWindow().UpdatesPerSecond = value;
            }
        }

        // Vertical Sync
        [ConVar("vsync")]
        public static bool VSync
        {
            get => GameWindow.CurrentWindow.GetSilkWindow().VSync;
            set
            {
                GameWindow.CurrentWindow.GetSilkWindow().VSync = value;
            }
        }

        // Cheats
        [ConVar("sv_cheats")]
        public static bool CheatsEnabled { get; set; } = false;
    }
}
