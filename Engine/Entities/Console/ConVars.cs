using SourceRewrite.Attributes;
using SourceRewrite.Windowing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
