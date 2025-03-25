using Editor.Components.GUI;
using Editor.Entities.GUI;
using SourceRewrite.Entities;
using SourceRewrite.Entities.GUI;
using SourceRewrite.InputSystem;

namespace Editor.Logic
{
    /// <summary>
    /// Handle Logic for the Main Map Operations Toolbar.
    /// </summary>
    public static class MainToolbar
    {
        public static GUICanvasEntity Canvas;

        // Track the Currently active Tool
        public static ToggleableTools CurrentTool = ToggleableTools.None;

        // Map Tools to Actions that run on left click
        private static Dictionary<ToggleableTools, Action> clickToolActions = new Dictionary<ToggleableTools, Action>()
        {
            // Tool Enum    |   Action to run on Left Click

            // ENTITY TOOL
            {ToggleableTools.Entity, () => {
                // Only create new Light Entity if one of the same name doesnt already exist
                if (EntityManager.MapContainer.FindInChildren("PointLight") != null)
                    return;

                // Create Point Light
                PointLight lightEntity = new PointLight();
                lightEntity.Color = new System.Numerics.Vector4(1, 10, 10, 10);
                lightEntity.Name = "PointLight";

                // Select Entity
                Selection.SelectedEntity = lightEntity;
                }
            }
        };

        // Map Tools to Actions that run on double click
        private static Dictionary<ToggleableTools, Action> doubleClickToolActions = new Dictionary<ToggleableTools, Action>()
        {
            // Tool Enum    |   Action to run on Left Click

            // ENTITY TOOL
            {ToggleableTools.Entity, () => {
                // Only open Entity Editor if it isnt already in the world
                if (EntityManager.MapContainer.FindInChildren("EntityEditor") != null)
                    return;

                // Open Entity Editor
                EntityEditorCanvas entityEditorCanvas = new EntityEditorCanvas();
                entityEditorCanvas.Name = "EntityEditor";
                entityEditorCanvas.Start();
                }
            }
        };



        // Register Events for the Toolbar
        public static void RegisterToolbarEvents()
        {
            Canvas.RegisterEvent("SetTool", (args) =>
            {
                if (args.Length > 0)
                {
                    // Attempt to parse the string from args[0] into a ToggleableTools enum
                    if (Enum.TryParse(args[0], true, out ToggleableTools tool))
                    {
                        CurrentTool = tool;
                        Console.WriteLine($"Tool set to: {CurrentTool}");
                    }
                    else
                    {
                        Console.WriteLine($"Invalid tool name: {args[0]}");
                    }
                }
            });

            // On Mouse Down
            Input.MouseButtonDownEvent += (mouse, button) =>
            {
                if (button == Silk.NET.Input.MouseButton.Left)
                {
                    // Handle tool usage
                    clickToolActions.TryGetValue(CurrentTool, out Action action);
                    action?.Invoke();
                }
            };

            // On Mouse Double Click
            Input.MouseDoubleClickEvent += (mouse, button, position) =>
            {
                if (button == Silk.NET.Input.MouseButton.Left)
                {
                    // Handle tool usage
                    doubleClickToolActions.TryGetValue(CurrentTool, out Action action);
                    action?.Invoke();
                }
            };
        }


        // Unregister Events for the Toolbar
        public static void UnregisterToolbarEvents()
        {
            Canvas.UnregisterEvent("SetTool");
        }
    }
}
