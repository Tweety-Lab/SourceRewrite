using SourceRewrite.Attributes;

namespace Game.Console
{
    public static class ConVars
    {
        [ConVar("my_convar", ConVarFlag.Save)]
        public static string MyConvar { get; set; }
    }
}
