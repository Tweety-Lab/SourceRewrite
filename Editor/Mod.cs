using Editor.Components.GUI;
using Editor.Components;
using SourceRewrite.Maps;
using SourceRewrite.Modding;
using SourceRewrite.Entities;

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
            // Run Entity start logic in our maps
            Map.StartEntitiesOnMapLoad = false;

            EditorCameraController editorCameraController = new EditorCameraController();
            EditorCanvas editorGUI = new EditorCanvas();

            EntityManager.AddGlobalEntity(editorCameraController);
            EntityManager.AddGlobalEntity(editorGUI);
        }

        // Runs once on Mod Unload
        public void OnUnload()
        {

        }
    }
}
