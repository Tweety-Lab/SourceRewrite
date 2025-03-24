using SourceRewrite.Attributes;
using SourceRewrite.Files;
using SourceRewrite.Maps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
