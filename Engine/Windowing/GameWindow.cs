using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.Windowing;
using SourceRewrite.Entities;
using SourceRewrite.Rendering;
using SourceRewrite.InputSystem;
using SourceRewrite.Files;
using SourceRewrite.Maps;
using System.Numerics;
using VistaGUI;
using FileFormats.KeyValues.GameInfo;
using SourceRewrite.TimeSystem;

// Application Window Instance that runs the engine in it, only one can exist at a time.
namespace SourceRewrite.Windowing
{
    public class GameWindow
    {
        public static GameWindow? CurrentWindow { get; private set; } // Active Game Window currently running
        public RendererContext Renderer { get; private set; } // Active Renderer 
        public InputContext Input { get; private set; } // Active Input Manager
        public GameInfoFormat GameInfo { get; private set; } // Active GameInfo.txt

        // Property to dynamically fetch the current window title
        public string WindowTitle
        {
            get
            {
                return _window.Title;
            }

            set
            {
                _window.Title = value;
            }
        }

        // Property to dynamically fetch the current window size
        public Vector2 WindowSize
        {
            get
            {
                Vector2D<int> currentSize = _window.Size;
                return new Vector2(currentSize.X, currentSize.Y);
            }

            set
            {
                Vector2D<int> newSize = new Vector2D<int>((int)value.X, (int)value.Y);
                _window.Size = newSize;
            }
        }

        private IWindow _window;

        // Init a Game Window
        public GameWindow(Vector2 windowSize, string windowTitle, string[] arguments = null)
        {
            // Update the Current Window
            CurrentWindow = this;

            // Process arguments only if there are any provided
            if (arguments != null)
                Application.ArgumentManager.SetArguments(arguments);

            Vector2D<int> trueWindowSize = new Vector2D<int>((int)windowSize.X, (int)windowSize.Y);

            // Set the options to match the constructor input
            WindowOptions options = WindowOptions.Default with
            {
                Size = trueWindowSize,
                Title = windowTitle
            };

            // Create the window
            _window = Window.Create(options);
            
            // Subscribe to window events
            _window.Load += OnLoad;
            _window.Update += OnUpdate;
            _window.Render += OnRender;
            _window.Closing += OnClose;
            _window.FramebufferResize += OnFramebufferResize;

            // Run the window
            _window.Run();

            _window.Dispose();

        }

        // Return the IWindow Context
        public IWindow GetSilkWindow()
        {
            return _window;
        }

        private unsafe async void OnLoad() {

            // Load GameInfo
            string gameInfoContent = File.ReadAllText("../../gameinfo.txt");
            GameInfo = new GameInfoFormat(gameInfoContent);

            // Set window title to game name as defined in GameInfo
            _window.Title = GameInfo.GameName;

            // Load Renderer (OpenGL)
            Renderer = new RendererContext(RendererAPI.OpenGL, this);
            Renderer.OnLoad();

            Renderer.SetClearColour(13, 13, 13, 255);

            // Load Input
            Input = new InputContext(_window.CreateInput());

            // Get -map window argument
            Application.Arguments.TryGetValue("-map", out string mapPath);
            if (mapPath != null)
            {
                MapSystem.LoadMap(FileSystem.GetMapPath(mapPath)); // Load Map from argument
            } else
            {
                MapSystem.LoadMap(FileSystem.GetMapPath("default.bsp")); // Load default map
            }
        }

        private void OnUpdate(double deltaTime) 
        {
            // Update Input
            Input.InputUpdate();

            // Update VistaGUI
            VistaContext.Update();

            // Update Time DeltaTime
            Time.DeltaTime = (float)deltaTime;

            // Update Entities
            EntityManager.EntityUpdate();
        }

        private unsafe void OnRender(double deltaTime) {
            // Render
            Renderer.OnRender();
        }

        private void OnFramebufferResize(Vector2D<int> newSize)
        {
            Renderer.OnFramebufferResize(newSize);
        }

        private void OnClose()
        {
            Renderer.OnClose();
        }
    }
}
