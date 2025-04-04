using SourceRewrite;
using SourceRewrite.Attributes;

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

        [ConCommand("print_convar")]
        static void PrintConvar()
        {
            DeveloperConsole.Msg($"Value of my_convar: {ConVars.MyConvar}");
        }
    }
}
