using Silk.NET.Maths;
using Silk.NET.Windowing;
using SourceRewrite.Entities;
using System.Numerics;
using VistaGUI;
using SourceRewrite.TimeSystem;
using SourceRewrite.Windowing.Modules;

// Application Window Instance that runs the engine in it, only one can exist at a time.
namespace SourceRewrite.Windowing
{
    public class GameWindow
    {
        public static GameWindow? CurrentWindow { get; private set; } // Active Game Window currently running

        public ModuleManager Modules = new ModuleManager();

        // Property to dynamically fetch the current window title
        public string WindowTitle
        {
            get => _window.Title;
            set => _window.Title = value;
        }

        // Property to dynamically fetch the current window size
        public Vector2 WindowSize
        {
            get => new Vector2(_window.Size.X, _window.Size.Y);
            set => _window.Size = new Vector2D<int>((int)value.X, (int)value.Y);
        }

        private readonly IWindow _window;

        public delegate void LoadAction();
        public delegate void UnloadAction();

        public LoadAction? OnLoadAction { get; set; }
        public UnloadAction? OnUnloadAction { get; set; }

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

        private void OnLoad()
        {
            // Register Modules, Order here DOES matter
            Modules.RegisterModule(new GameInfoModule());
            Modules.RegisterModule(new RenderModule());
            Modules.RegisterModule(new InputModule());
            Modules.RegisterModule(new EntityModule());
            Modules.RegisterModule(new MapModule());

            // Invoke OnLoadAction
            OnLoadAction?.Invoke();
        }

        private void OnUpdate(double deltaTime)
        {
            Modules.UpdateModules(deltaTime);

            // Update VistaGUI
            VistaContext.Update();

            // Update Time DeltaTime
            Time.DeltaTime = (float)deltaTime;

            // Update Entities
            EntityManager.UpdateAllEntities();
        }

        private unsafe void OnRender(double deltaTime)
        {
            // Render
            Modules.RenderModules(deltaTime);
        }

        private void OnFramebufferResize(Vector2D<int> newSize)
        {
            Modules.ResizeModules(newSize);
        }

        private void OnClose()
        {
            Modules.ShutdownModules();

            // Invoke OnUnloadAction
            OnUnloadAction?.Invoke();
        }
    }
}
