using SourceRewrite.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Console
{
    public static class ConVars
    {
        [ConVar("my_convar")]
        public static string MyConvar { get; set; }
    }
}
