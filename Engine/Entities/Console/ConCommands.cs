using SourceRewrite.Attributes;
using SourceRewrite.Files;
using SourceRewrite.Maps;

namespace SourceRewrite.Entities.Console
{
    public static class ConCommands
    {
        [ConCommand("map")]
        static void Map(string path)
        {
            MapSystem.LoadMap(FileSystem.GetMapPath(path));
        }
    }
}
