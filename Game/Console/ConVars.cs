using SourceRewrite.Attributes;

namespace Game.Console
{
    public static class ConVars
    {
        [ConVar("my_convar", ConVarFlag.Archive)]
        public static string MyConvar { get; set; }
    }
}
