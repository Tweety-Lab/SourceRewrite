using Editor.Components.GUI;
using SourceRewrite.Maps;
using SourceRewrite.Modding;
using SourceRewrite.Objects;
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

            GameObject editorGUIObject = new GameObject();

            // Load the Editor GUI
            EditorCanvas editorGUIComponent = new EditorCanvas();

            editorGUIObject.AddComponent(editorGUIComponent);

            Map.GlobalGameObjects.Add(editorGUIObject);

            // Create our main Game Window
            new GameWindow(new Vector2(800, 500), "Game Window");
        }
    }
}