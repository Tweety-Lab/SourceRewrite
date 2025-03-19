using SourceRewrite.Modding;
using SourceRewrite.Windowing;
using System.Numerics;

namespace Editor
{
    class Program
    {
        static void Main(string[] args)
        {
            // Load Editor as Mod by default
            ModSystem.LoadModAssembly("Editor.dll");

            // Load Game as Mod
            ModSystem.LoadModAssembly("Game.dll");

            // Load Mods from bin/plugins
            ModSystem.LoadModsFromDir("../editor-plugins/");

            // Create our main Game Window
            new GameWindow(new Vector2(800, 500), "Game Window");
        }
    }
}