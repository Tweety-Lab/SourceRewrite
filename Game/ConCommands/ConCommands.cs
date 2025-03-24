using SourceRewrite;
using SourceRewrite.Attributes;
using SourceRewrite.Maps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.ConCommands
{
    public static class ConCommands
    {
        static int pingCount = 0;

        [ConCommand("ping_client")]
        static void Ping()
        {
            EngineConsole.Msg($"Pong! You have run this command {++pingCount} times.");
        }

        [ConCommand("echo")]
        static void Echo(string input)
        {
            EngineConsole.Msg(input);
        }
    }
}
