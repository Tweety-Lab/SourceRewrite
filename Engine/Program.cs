using SourceRewrite.Modding;
using SourceRewrite.Windowing;
using System.Numerics;

// Entry Point
class Program
{
    static void Main()
    {
        // Load Game.dll as a Mod
        ModSystem.LoadModAssembly("Game.dll");

        // Create our main Game Window
        new GameWindow(new Vector2(800, 500), "Game Window");
    }
}
