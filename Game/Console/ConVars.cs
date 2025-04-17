using SourceRewrite.Attributes;

namespace Game.ConsoleVariables
{
    public static class ConVars
    {
        [ConVar("my_convar", ConVarFlag.Archive)]
        public static string MyConvar { get; set; }
    }
}
