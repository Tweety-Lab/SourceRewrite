using Editor.Components.GUI;
using Editor.Components;
using SourceRewrite.Maps;
using SourceRewrite.Modding;
using SourceRewrite.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SourceRewrite.Components;

namespace Editor
{
    /// <summary>
    /// Entry Point for Editor Mod.
    /// </summary>
    public class EditorMod : IMod
    {
        // Runs once on Mod Load
        public void OnLoad()
        {
            // Dont run GameObject start logic in our maps
            Map.StartGameObjectsOnMapLoad = false;

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
        }

        // Runs once on Mod Unload
        public void OnUnload()
        {

        }
    }
}
