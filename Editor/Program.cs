using Editor.Components;
using Editor.Components.GUI;
using SourceRewrite.Maps;
using SourceRewrite.Modding;
using SourceRewrite.Objects;
using SourceRewrite.Windowing;
using SourceRewrite.Components;
using System.Numerics;

namespace Editor
{
    class Program
    {
        static void Main(string[] args)
        {
            // Load Editor as Mod by default
            ModSystem.LoadModAssembly("Editor.dll");

            // Load Mods from bin/plugins
            ModSystem.LoadModsFromDir("../editor-plugins/");

            // Create our main Game Window
            new GameWindow(new Vector2(800, 500), "Game Window");
        }
    }
}