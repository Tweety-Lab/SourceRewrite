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
            EditorCameraController editorCameraController = new EditorCameraController();
            EditorCanvas editorGUI = new EditorCanvas();

            EntityManager.AddGlobalEntity(editorCameraController);
            EntityManager.AddGlobalEntity(editorGUI);

            // Make sure Entity Logic in maps don't run
            MapSystem.OnMapPreload += (map) => map.EntitiesEnabled = false;

            // Render All Gizmos
            MapSystem.OnMapLoaded += (map) =>
            {
                foreach (BaseEntity entity in map.Entities)
                {
                    entity.OnDrawGizmos();
                }
            };
        }

        // Runs once on Mod Unload
        public void OnUnload()
        {

        }
    }
}
