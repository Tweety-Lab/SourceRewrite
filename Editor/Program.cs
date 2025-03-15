using SourceRewrite.Modding;
using SourceRewrite.Windowing;
using System.Numerics;

namespace Editor
{
    class Program
    {
        static void Main(string[] args)
        {
            // Load Editor Code
            ModSystem.LoadModModule("Editor.dll");

            // Create our main Game Window
            new GameWindow(new Vector2(800, 500), "Game Window");
        }
    }
}