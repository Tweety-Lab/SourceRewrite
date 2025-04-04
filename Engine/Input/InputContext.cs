using Silk.NET.Input;
using SourceRewrite.Attributes;
using SourceRewrite.Windowing.Modules;
using System.Numerics;

namespace SourceRewrite.InputSystem
{
    public class InputContext
    {
        public Dictionary<string, string> Keybinds = new(); // <Action, Key>

        // Keybind Map
        public Dictionary<string, Key> KeybindMap = CreateKeybindMap();

        public IKeyboard PrimaryKeyboard;
        public IMouse PrimaryMouse;

        /// <summary>
        /// Mouse Movement since last Frame.
        /// </summary>
        public Vector2 MouseDelta;

        private Vector2 lastMousePos;

        public InputContext(IInputContext input)
        {
            PrimaryKeyboard = input.Keyboards.FirstOrDefault();
            PrimaryMouse = input.Mice.FirstOrDefault();
            lastMousePos = PrimaryMouse.Position; // Initialize last mouse position
        }

        public void InputUpdate()
        {
            // Calculate and update MouseDelta
            MouseDelta = PrimaryMouse.Position - lastMousePos;
            lastMousePos = PrimaryMouse.Position;
        }

        private static Dictionary<string, Key> CreateKeybindMap()
        {
            return new Dictionary<string, Key>
            {
                // Alphanumeric Keys (0-9, A-Z)
                { "0", Key.Number0 },
                { "1", Key.Number1 },
                { "2", Key.Number2 },
                { "3", Key.Number3 },
                { "4", Key.Number4 },
                { "5", Key.Number5 },
                { "6", Key.Number6 },
                { "7", Key.Number7 },
                { "8", Key.Number8 },
                { "9", Key.Number9 },
                { "a", Key.A },
                { "b", Key.B },
                { "c", Key.C },
                { "d", Key.D },
                { "e", Key.E },
                { "f", Key.F },
                { "g", Key.G },
                { "h", Key.H },
                { "i", Key.I },
                { "j", Key.J },
                { "k", Key.K },
                { "l", Key.L },
                { "m", Key.M },
                { "n", Key.N },
                { "o", Key.O },
                { "p", Key.P },
                { "q", Key.Q },
                { "r", Key.R },
                { "s", Key.S },
                { "t", Key.T },
                { "u", Key.U },
                { "v", Key.V },
                { "w", Key.W },
                { "x", Key.X },
                { "y", Key.Y },
                { "z", Key.Z },


                // Modifier Keys
                { "space", Key.Space },
                { "ctrl", Key.ControlLeft },
                { "shift", Key.ShiftLeft },
                { "alt", Key.AltLeft },
                { "tab", Key.Tab },
                { "enter", Key.Enter },
                { "escape", Key.Escape },
                { "capslock", Key.CapsLock },
                { "numlock", Key.NumLock },
                { "scrolllock", Key.ScrollLock },
                { "rshift", Key.ShiftRight },
                { "rctrl", Key.ControlRight },
                { "ralt", Key.AltRight },

                // Function Keys
                { "f1", Key.F1 },
                { "f2", Key.F2 },
                { "f3", Key.F3 },
                { "f4", Key.F4 },
                { "f5", Key.F5 },
                { "f6", Key.F6 },
                { "f7", Key.F7 },
                { "f8", Key.F8 },
                { "f9", Key.F9 },
                { "f10", Key.F10 },
                { "f11", Key.F11 },
                { "f12", Key.F12 },

                // Navigation Keys
                { "uparrow", Key.Up },
                { "downarrow", Key.Down },
                { "leftarrow", Key.Left },
                { "rightarrow", Key.Right },
                { "ins", Key.Insert },
                { "del", Key.Delete },
                { "pgdn", Key.PageDown },
                { "pgup", Key.PageUp },
                { "home", Key.Home },
                { "end", Key.End },
                { "pause", Key.Pause },

                // Special Keys
                { "backspace", Key.Backspace },
                { ";", Key.Semicolon },
                { "/", Key.Slash },
                { ",", Key.Comma },
                { ".", Key.Period },
                { "'", Key.Apostrophe },
                { "[", Key.LeftBracket },
                { "]", Key.RightBracket },
                { "\\", Key.BackSlash },
                { "=", Key.Equal },
                { "`", Key.GraveAccent }
            };
        }
    }

    /// <summary>
    /// Input Abstraction.
    /// </summary>
    public static class Input
    {
        // Events
        public static event Action<IKeyboard, Key, int> KeyDownEvent;
        public static event Action<IKeyboard, Key, int> KeyUpEvent;
        public static event Action<IKeyboard, char> KeyCharEvent;
        public static event Action<IMouse, MouseButton> MouseButtonDownEvent;
        public static event Action<IMouse, MouseButton> MouseButtonUpEvent;
        public static event Action<IMouse, MouseButton, Vector2> MouseDoubleClickEvent;

        private static InputContext Context => GameModules.GetModule<InputModule>().Context;

        static Input()
        {
            InitializeInputEvents();
        }

        private static void InitializeInputEvents()
        {
            if (Context.PrimaryKeyboard != null)
            {
                Context.PrimaryKeyboard.KeyDown += OnKeyDown;
                Context.PrimaryKeyboard.KeyUp += OnKeyUp;
                Context.PrimaryKeyboard.KeyChar += OnKeyChar;
            }

            if (Context.PrimaryMouse != null)
            {
                Context.PrimaryMouse.MouseDown += OnMouseButtonDown;
                Context.PrimaryMouse.MouseUp += OnMouseButtonUp;
                Context.PrimaryMouse.DoubleClick += OnMouseDoubleClick;
            }
        }

        // Key Handlers
        private static void OnKeyDown(IKeyboard sender, Key key, int i) => KeyDownEvent?.Invoke(sender, key, i);
        private static void OnKeyUp(IKeyboard sender, Key key, int i) => KeyUpEvent?.Invoke(sender, key, i);
        private static void OnKeyChar(IKeyboard sender, char character) => KeyCharEvent?.Invoke(sender, character);

        // Mouse Handlers
        private static void OnMouseButtonDown(IMouse sender, MouseButton button) => MouseButtonDownEvent?.Invoke(sender, button);
        private static void OnMouseButtonUp(IMouse sender, MouseButton button) => MouseButtonUpEvent?.Invoke(sender, button);
        private static void OnMouseDoubleClick(IMouse sender, MouseButton button, Vector2 position) => MouseDoubleClickEvent?.Invoke(sender, button, position);

        // Key Functions
        public static bool GetKeyDown(Key key) => Context.PrimaryKeyboard.IsKeyPressed(key);
        public static bool GetKeyUp(Key key) => !GetKeyDown(key);

        // Mouse Functions
        public static bool GetMouseButtonDown(int mouseButton) => Context.PrimaryMouse.IsButtonPressed((Silk.NET.Input.MouseButton)mouseButton);
        public static bool GetMouseButtonUp(int mouseButton) => !GetMouseButtonDown(mouseButton);
        public static Vector2 GetMousePosition() => Context.PrimaryMouse.Position;
        public static float GetMouseX() => Context.PrimaryMouse.Position.X;
        public static float GetMouseY() => Context.PrimaryMouse.Position.Y;
        public static Vector2 GetMouseMovement() => Context.MouseDelta;
        public static float GetMouseXMovement() => Context.MouseDelta.X;
        public static float GetMouseYMovement() => Context.MouseDelta.Y;

        public static string GetClipboardText() => Context.PrimaryKeyboard.ClipboardText;

        // Cursor Lock/Unlock
        public static void LockCursor() => Context.PrimaryMouse.Cursor.CursorMode = CursorMode.Hidden;
        public static void UnlockCursor() => Context.PrimaryMouse.Cursor.CursorMode = CursorMode.Normal;

        // Binding Functions
        [ConCommand("bind")]
        public static void BindKey(string key, string action) => Context.Keybinds[action] = key.ToLower();

        /// <summary>
        /// Checks if an action is down.
        /// </summary>
        /// <param name="action"></param>
        public static bool GetDown(string action)
        {
            if (Context.Keybinds.TryGetValue(action, out var keyString) &&
                Context.KeybindMap.TryGetValue(keyString, out var key))
            {
                return Context.PrimaryKeyboard.IsKeyPressed(key);
            }
            return false;
        }

        /// <summary>
        /// Checks if an action is up.
        /// </summary>
        /// <param name="action"></param>
        public static bool GetUp(string action) => !GetDown(action);

        /// <summary>
        /// Returns a list of all bound actions.
        /// </summary>
        public static List<string> GetBoundActions()
        {
            var boundActions = new List<string>();

            foreach (var keybind in Context.Keybinds)
            {
                // Get the action (key) and its associated key (as a string)
                var action = keybind.Key;
                var key = keybind.Value;

                // Add the action and associated key to the list in string form
                boundActions.Add($"{key} {action}");
            }

            return boundActions;
        }
    }
}
