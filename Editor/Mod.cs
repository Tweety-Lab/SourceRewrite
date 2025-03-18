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
            GameObject editorGUIObject = new GameObject("EditorGUI");
            EditorCanvas editorGUIComponent = new EditorCanvas();
            editorGUIObject.AddComponent(editorGUIComponent);

            // Load Editor Camera
            GameObject editorCameraObject = new GameObject("EditorCamera");
            Camera editorCamera = new Camera();
            EditorCameraController editorCameraController = new EditorCameraController();
            editorCameraObject.AddComponent(editorCameraController);
            editorCameraObject.AddComponent(editorCamera);

            GameObjectManager.AddGlobalObject(editorGUIObject);
            GameObjectManager.AddGlobalObject(editorCameraObject);


        }

        // Runs once on Mod Unload
        public void OnUnload()
        {

        }
    }
}
