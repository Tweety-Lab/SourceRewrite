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
            // Load Editor as Mod
            ModSystem.LoadModAssembly("Editor.dll");

            // Load the Editor GUI
            GameObject editorGUIObject = new GameObject();
            EditorCanvas editorGUIComponent = new EditorCanvas();
            editorGUIObject.AddComponent(editorGUIComponent);

            // Add Editor GUI as global component
            Map.GlobalGameObjects.Add(editorGUIObject);

            // Load Editor Camera
            GameObject editorCameraObject = new GameObject();
            Camera editorCamera = new Camera();
            EditorCameraController editorCameraController = new EditorCameraController();
            editorCameraObject.AddComponent(editorCameraController);
            editorCameraObject.AddComponent(editorCamera);

            // Add Editor Camera as global component
            Map.GlobalGameObjects.Add(editorCameraObject);


            // Create our main Game Window
            new GameWindow(new Vector2(800, 500), "Game Window");
        }
    }
}