using Silk.NET.Input;
using SourceRewrite.AssetTypes;
using SourceRewrite.Attributes;
using SourceRewrite.Files;
using SourceRewrite.GUI;
using SourceRewrite.InputSystem;
using SourceRewrite.Maps;
using SourceRewrite.Rendering;
using SourceRewrite.Windowing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using VistaGUI;
using VistaGUI.Scripting.References;

namespace SourceRewrite.Entities.GUI
{
    public class GUICanvasEntity : BaseEntity
    {
        [EntityProperty("height")]
        private int height; // Panel Height

        [EntityProperty("width")]
        private int width; // Panel Width

        [EntityProperty("panelname")]
        public string PanelName; // Name of HTML Panel to render

        [EntityProperty("IsTransparent")]
        public bool IsTransparent; // Is the panel background transparent

        public int PanelType = 1; // Type of Panel, 0 = worldspace, 1 = screenspace.

        public GUIContainer Container;

        // Worldspace rendering
        private MeshAsset guiMesh;

        public override void Start()
        {
            // Create view config
            GUIConfig config = new GUIConfig();

            config.ResourcesPath = Path.GetFullPath(FileSystem.GamePath.GUIPath);

            config.IsTransparent = IsTransparent;
            config.EnableJavaScript = true;

            int resolutionDensity = 12;
            if (PanelType != 0)
            {
                resolutionDensity = 1;
                width = (int)GameWindow.CurrentWindow.WindowSize.X;
                height = (int)GameWindow.CurrentWindow.WindowSize.Y;
            }

            VistaView vistaView = new VistaView(PanelName, config, resolutionDensity, height, width);

            Container = new GUIContainer();

            // Create a GUI View
            Container.VistaView = vistaView;

            // Send Character Typed Events
            Input.KeyCharEvent += (sender, character) =>
            {
                Container.VistaView.SendText(character.ToString(), false);
            };

            // Send Key Down Events
            Input.KeyDownEvent += (sender, key, i) =>
            {
                bool isShiftPressed = Input.GetKeyDown(Key.ShiftLeft) || Input.GetKeyDown(Key.ShiftRight);
                bool isCtrlPressed = Input.GetKeyDown(Key.ControlLeft) || Input.GetKeyDown(Key.ControlRight);
                string keyText = key.ToString();

                // Support pasting from Keyboard
                if (isCtrlPressed && key == Key.V)
                {
                    keyText = Input.GetClipboardText(); // Get copied text
                    Container.VistaView.SendText(keyText, true); // Send copied text to GUI
                    return;
                }

                Container.VistaView.SendKeyDown(i, isShiftPressed ? VistaKeyModifiers.ShiftKey : VistaKeyModifiers.None, keyText);
            };

            // Send Key Ups Events
            Input.KeyUpEvent += (sender, key, i) =>
            {
                bool isShiftPressed = Input.GetKeyDown(Key.ShiftLeft) || Input.GetKeyDown(Key.ShiftRight);
                string keyText = key.ToString();

                Container.VistaView.SendKeyUp(i, isShiftPressed ? VistaKeyModifiers.ShiftKey : VistaKeyModifiers.None, keyText);
            };

            // Send Mouse Down Events
            Input.MouseButtonDownEvent += (sender, button) =>
            {
                if (button == MouseButton.Left)
                {
                    Container.VistaView.SendMouseButtonDown(0);
                }
                else
                {
                    Container.VistaView.SendMouseButtonDown(1);
                }
            };

            // Send Mouse Up events
            Input.MouseButtonUpEvent += (sender, button) =>
            {
                if (button == MouseButton.Left)
                {
                    Container.VistaView.SendMouseButtonUp(0);
                }
                else
                {
                    Container.VistaView.SendMouseButtonUp(1);
                }
            };


            // Render view depending on if it's worldspace or screenspace
            if (PanelType == 0)
            {
                RenderViewToObject();
            }
        }

        public override void Update()
        {
            // Get current mouse position
            var mousePosition = Input.GetMousePosition();

            Container.VistaView.SendMousePosition(mousePosition);

            // Render view
            Texture texture = Container.RenderToTexture();

            // Update view depending on if it's worldspace or screenspace
            if (PanelType == 0)
            {
                if (guiMesh != null)
                    guiMesh.Material.Texture = texture;
            }
        }

        /// <summary>
        /// Gets an Element from it's ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public VistaElement GetElement(string id) => Container.VistaView.ScriptingContext.GetElement(id);

        /// <summary>
        /// Gets an Element from it's ID as a specific type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        public T GetElementAsType<T>(string id) where T : VistaElement
        {
            return Container.VistaView.ScriptingContext.GetElementAsType<T>(id);
        }

        /// <summary>
        /// Registers a C# Action that can be called from JavaScript with arguments.
        /// </summary>
        /// <param name="name">JavaScript function name</param>
        /// <param name="action">C# Action that accepts arguments (optional)</param>
        public void RegisterEvent(string name, Action<string[]> action) => Container.VistaView.ScriptingContext.RegisterEvent(name, action);

        /// <summary>
        /// Registers a C# Action that can be called from JavaScript.
        /// </summary>
        /// <param name="name">JavaScript function name</param>
        public void RegisterEvent(string name, Action action) => Container.VistaView.ScriptingContext.RegisterEvent(name, action);

        /// <summary>
        /// Unregisters a C# Action that can be called from JavaScript.
        /// </summary>
        /// <param name="name">Javascript function name</param>
        public void UnregisterEvent(string name) => Container.VistaView.ScriptingContext.UnregisterEvent(name);


        // Render a GUIView in worldspace
        private void RenderViewToObject()
        {
            // Implement later
        }
    }
}
