using SourceRewrite.AssetTypes;
using SourceRewrite.Components;
using SourceRewrite.Files;
using SourceRewrite.InputSystem;
using SourceRewrite.Objects;
using System.Diagnostics;
using System;
using System.Numerics;
using VistaGUI.Scripting.References;
using SourceRewrite.Maps;
using SourceRewrite.Components.GUI;
using System.Text;

namespace Editor.Components.GUI
{
    public class EditorCanvas : GameComponent
    {
        private GUICanvas canvas;

        public override void Start()
        {
            Console.WriteLine("Started Editor GUI");

            canvas = new GUICanvas();
            canvas.IsTransparent = true;
            canvas.PanelName = "editor/editor.html";

            GameObject.AddComponent(canvas);

            // HACK: Manually start the canvas component
            canvas.Start();

            // Run Code on Map Load
            MapSystem.OnMapLoaded += (Map map) =>
            {
                UpdateGameObjectsTree(); // Update GUI GameObjects Tree
            };

            // Register GUI Events
            RegisterGUIEvents();
        }

        public override void Update()
        {

        }

        public override void OnDestroy()
        {
            // Unregister all events
            canvas.UnregisterEvent("OpenVDC");
            canvas.UnregisterEvent("OpenLightEditor");
            canvas.UnregisterEvent("CloseLightEditor");
            canvas.UnregisterEvent("CloseMap");
            canvas.UnregisterEvent("PlayGame");
            canvas.UnregisterEvent("PlayCurrentMap");
            canvas.UnregisterEvent("OpenMap");
            canvas.UnregisterEvent("NewGameObject");

        }

        // Load GameObjects from a Map into GUI gameobjects tree
        private void UpdateGameObjectsTree()
        {
            // Load GameObjects into gui gameobjects list
            VistaUnorderedList gameobjectsList = canvas.GetElementAsType<VistaUnorderedList>("gameobjects-list");

            gameobjectsList.Clear();
            foreach (GameObject gameobject in MapSystem.CurrentMap.GameObjects)
            {
                // Dont list global game objects
                if (MapSystem.GlobalGameObjects.Contains(gameobject))
                    continue;

                // Set the name to the first non-transform component name
                gameobjectsList.AddListItem(gameobject.Name ?? "NameNotFound");
            }
        }

        // Register GUI Events
        private void RegisterGUIEvents()
        {
            RegisterMenuEvents();
            RegisterGameObjectEvents();
        }

        private void RegisterMenuEvents()
        {
            // Open VDC Website
            canvas.RegisterEvent("OpenVDC", () => Process.Start(new ProcessStartInfo("https://developer.valvesoftware.com/wiki/Main_Page") { UseShellExecute = true }));

            // Unload Map
            canvas.RegisterEvent("CloseMap", () => MapSystem.UnloadMap());

            // Play Buttons
            canvas.RegisterEvent("PlayGame", () => Process.Start("Engine.exe")); // Just start Engine.exe as it loads game.dll

            // Start Engine.exe with the current map as an argument
            canvas.RegisterEvent("PlayCurrentMap", () =>
            {
                string mapFileName = MapSystem.CurrentMap.BSPFilePath.Split('/').Last(); // Get the file name of the map
                Process.Start("Engine.exe", $"-map {mapFileName}"); // Start the process with the map argument
            });

            // Allow user to select map file then open it
            canvas.RegisterEvent("OpenMap", () =>
            {
                // Create gameobject to house fileExplorer GUICanvas
                GameObject fileExplorerObject = new GameObject();

                // Add GUICanvas to gameobject
                FileExplorerCanvas fileExplorerCanvas = new FileExplorerCanvas();
                fileExplorerObject.AddComponent(fileExplorerCanvas);

                // Start GUI
                fileExplorerCanvas.Start();
            });
        }

        private void RegisterGameObjectEvents()
        {
            // Create a new game object
            canvas.RegisterEvent("NewGameObject", () =>
            {
                GameObject gameObject = new GameObject();
                gameObject.Name = "NewGameObject";

                MapSystem.CurrentMap.GameObjects.Add(gameObject);

                // Update gameobjects tree
                VistaUnorderedList gameobjectsList = canvas.GetElementAsType<VistaUnorderedList>("gameobjects-list");
                gameobjectsList.AddListItem(gameObject.Name ?? "NameNotFound");
            });

            // Delete most recent game object
            canvas.RegisterEvent("DeleteGameObject", () =>
            {
                // Loop through every game object in reverse order (back to front)
                for (int i = MapSystem.CurrentMap.GameObjects.Count - 1; i >= 0; i--)
                {
                    GameObject gameObject = MapSystem.CurrentMap.GameObjects[i];

                    if (!MapSystem.GlobalGameObjects.Contains(gameObject))
                    {
                        // Destroy and remove the first non-global game object found
                        MapSystem.CurrentMap.GameObjects.RemoveAt(i);
                        gameObject.DestroyDeferred();
                        break;
                    }
                }

                UpdateGameObjectsTree();
            });
        }
    }
}
