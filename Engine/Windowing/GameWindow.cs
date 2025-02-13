using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.Windowing;
using SourceRewrite.Components;
using SourceRewrite.Objects;
using SourceRewrite.Rendering;
using System.Reflection;
using SourceRewrite.InputSystem;
using SourceRewrite.FileSystem;

// Application Window Instance that runs the engine in it, only one can exist at a time.
namespace SourceRewrite.Windowing
{
    public class GameWindow
    {
        public static GameWindow? CurrentWindow { get; private set; } // Active Game Window currently running
        public RendererContext Renderer { get; private set; } // Active Renderer 
        public InputContext Input { get; private set; } // Active Input Manager
        public GameInfoContext GameInfo { get; private set; } // Active GameInfo.txt

        public string WindowTitle { get; private set; }
        public Vector2D<int> WindowSize { get; private set; }

        private IWindow _window;

        // Init a Game Window
        public GameWindow(Vector2D<int> windowSize, string windowTitle)
        {
            // Update the Current Window
            CurrentWindow = this;

            // Set the options to match the constructor input
            WindowOptions options = WindowOptions.Default with
            {
                Size = windowSize,
                Title = windowTitle
            };

            // Create the window
            _window = Window.Create(options);

            WindowTitle = windowTitle;
            WindowSize = windowSize;
            
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
            GameInfo = new GameInfoContext("../../gameinfo.txt");

            // Load Renderer (OpenGL)
            Renderer = new RendererContext(RendererAPI.OpenGL, this);
            Renderer.OnLoad();

            Renderer.SetClearColour(13, 13, 13, 255);

            // Load Input
            Input = new InputContext(_window.CreateInput());

            // Load GameObjects
            GameObject.GameObjectStart();
        }

        private void OnUpdate(double deltaTime) 
        {
            // Update Input
            Input.InputUpdate();

            // Update GameObjects
            GameObject.GameObjectUpdate(deltaTime);
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
