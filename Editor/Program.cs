using Editor.Components.GUI;
using SourceRewrite.Maps;
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
            ModSystem.LoadModAssembly("Editor.dll");

            // Load the Editor GUI as a Global Component
            EditorCanvas globalComponent = new EditorCanvas();
            Map.GlobalComponents.Add(globalComponent);

            // Create our main Game Window
            new GameWindow(new Vector2(800, 500), "Game Window");
        }
    }
}