using Editor.Components.GUI;
using SourceRewrite.Entities;
using SourceRewrite.Entities.GUI;
using SourceRewrite.InputSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Entities.GUI
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
        private static Dictionary<ToggleableTools, Action> toolActions = new Dictionary<ToggleableTools, Action>()
        {
            // Tool Enum    |   Action to run on Left Click

            // ENTITY TOOL
            {ToggleableTools.Entity, () => {
                // Only create new Light Entity if one of the same name doesnt already exist
                if (EntityManager.MapContainer.FindInChildren("PointLight") != null)
                    return;

                // Create Point Light
                PointLight lightEntity = new PointLight();
                lightEntity.Color = new System.Numerics.Vector4(100, 100, 100, 200);
                lightEntity.Name = "PointLight";
                lightEntity.Parent = EntityManager.MapContainer.Children[0]; // Add to current map
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
                    toolActions.TryGetValue(CurrentTool, out Action action);
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
