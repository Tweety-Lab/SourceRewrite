using SourceRewrite.Attributes;

namespace Game.Console
{
    public static class ConVars
    {
        [ConVar("my_convar")]
        public static string MyConvar { get; set; }
    }
}
