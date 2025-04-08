using Silk.NET.Input;
using SourceRewrite.Attributes;
using SourceRewrite.InputSystem;
using SourceRewrite.Windowing.Modules;
using System.Numerics;

namespace SourceRewrite.InputSystem
{
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
        public static void BindKey(string key, string action)
        {
            // Remove Source Engine keybind prefixes
            if (action.StartsWith('+') || action.StartsWith('-'))
            {
                action = action.Substring(1);
            }

            Context.Keybinds[action.Replace('+', ' ').Replace('-', ' ')] = key.ToLower();
        }

        [ConCommand("unbind")]
        public static void UnbindKey(string action) => Context.Keybinds.Remove(action);

        [ConCommand("unbindall")]
        public static void UnbindAllKeys() => Context.Keybinds.Clear();

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
