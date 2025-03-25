using SourceRewrite;
using SourceRewrite.Attributes;
using SourceRewrite.Maps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Console
{
    public static class ConCommands
    {
        static int pingCount = 0;

        [ConCommand("ping_client")]
        static void Ping()
        {
            DeveloperConsole.Msg($"Pong! You have run this command {++pingCount} times.");
        }

        [ConCommand("echo")]
        static void Echo(string input)
        {
            DeveloperConsole.Msg(input);
        }

        [ConCommand("print_convar")]
        static void PrintConvar()
        {
            DeveloperConsole.Msg($"Value of my_convar: {ConVars.MyConvar}");
        }
    }
}
