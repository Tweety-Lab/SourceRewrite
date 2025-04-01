using Silk.NET.Input;
using SourceRewrite.Windowing;
using SourceRewrite.Windowing.Modules;
using System.Numerics;

namespace SourceRewrite.InputSystem
{
    public class InputContext
    {
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

            // Initialize lastMousePos
            lastMousePos = PrimaryMouse.Position;
        }

        public void InputUpdate()
        {
            // Calculate the difference (delta) between frames
            MouseDelta = PrimaryMouse.Position - lastMousePos;

            // Store the current mouse position for the next frame
            lastMousePos = PrimaryMouse.Position;
        }
    }

    /// <summary>
    /// Input Abstraction.
    /// </summary>
    public static class Input
    {
        // KeyDown Event
        public static event Action<IKeyboard, Key, int> KeyDownEvent;

        // KeyUp Event
        public static event Action<IKeyboard, Key, int> KeyUpEvent;

        // Char Event
        public static event Action<IKeyboard, char> KeyCharEvent;

        // MouseButtonDown Event
        public static event Action<IMouse, MouseButton> MouseButtonDownEvent;

        // MouseButtonUp Event
        public static event Action<IMouse, MouseButton> MouseButtonUpEvent;

        // MouseDoubleClick Event
        public static event Action<IMouse, MouseButton, Vector2> MouseDoubleClickEvent;

        private static InputContext Context => GameWindow.CurrentWindow.Modules.GetModule<InputModule>().Context;

        static Input()
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

        // Handler for the KeyDown event
        private static void OnKeyDown(IKeyboard sender, Key key, int i)
        {
            KeyDownEvent?.Invoke(sender, key, i);
        }

        // Handler for the KeyUp event
        private static void OnKeyUp(IKeyboard sender, Key key, int i)
        {
            KeyUpEvent?.Invoke(sender, key, i);
        }

        // Handler for the KeyChar event
        private static void OnKeyChar(IKeyboard sender, char character)
        {
            KeyCharEvent?.Invoke(sender, character);
        }

        // Handler for the MouseButtonDown event
        private static void OnMouseButtonDown(IMouse sender, MouseButton button)
        {
            MouseButtonDownEvent?.Invoke(sender, button);
        }

        // Handler for the MouseButtonUp event
        private static void OnMouseButtonUp(IMouse sender, MouseButton button)
        {
            MouseButtonUpEvent?.Invoke(sender, button);
        }

        // Handler for the MouseDoubleClick event
        private static void OnMouseDoubleClick(IMouse sender, MouseButton button, Vector2 position)
        {
            MouseDoubleClickEvent?.Invoke(sender, button, position);
        }

        /// <summary>
        /// Returns True if the chosen key is pressed down.
        /// </summary>
        public static bool GetKeyDown(Key key)
        {
            return Context.PrimaryKeyboard.IsKeyPressed(key);
        }

        

        /// <summary>
        /// Returns True if the chosen key is up.
        /// </summary>
        public static bool GetKeyUp(Key key)
        {
            return !Context.PrimaryKeyboard.IsKeyPressed(key);
        }

        /// <summary>
        /// Returns True if chosen Mouse Button is down.
        /// </summary>
        public static bool GetMouseButtonDown(int mouseButton)
        {
            // Convert the int to a Silk.NET MouseButton
            MouseButton button = (Silk.NET.Input.MouseButton)mouseButton;

            // Check if the specific mouse button is pressed
            return Context.PrimaryMouse.IsButtonPressed(button);
        }

        /// <summary>
        /// Returns True if chosen Mouse Button is up.
        /// </summary>
        public static bool GetMouseButtonUp(int mouseButton)
        {
            // Check if the specific mouse button is not pressed
            return !GetMouseButtonDown(mouseButton);
        }

        /// <summary>
        /// Returns current Mouse Position as Vector2.
        /// </summary>
        public static Vector2 GetMousePosition()
        {
            return Context.PrimaryMouse.Position;
        }

        /// <summary>
        /// Returns current Mouse X Position.
        /// </summary>
        public static float GetMouseX()
        {
            return Context.PrimaryMouse.Position.X;
        }

        /// <summary>
        /// Returns current Mouse Y Position.
        /// </summary>
        public static float GetMouseY()
        {
            return Context.PrimaryMouse.Position.Y;
        }

        /// <summary>
        /// Returns Mouse Movement since last Frame.
        /// </summary>
        public static Vector2 GetMouseMovement()
        {
            return Context.MouseDelta;
        }

        /// <summary>
        /// Returns X Mouse Movement since last Frame.
        /// </summary>
        public static float GetMouseXMovement()
        {
            return Context.MouseDelta.X;
        }

        /// <summary>
        /// Returns Y Mouse Movement since last Frame.
        /// </summary>
        public static float GetMouseYMovement()
        {
            return Context.MouseDelta.Y;
        }

        /// <summary>
        /// Returns the current Clipboard Text.
        /// </summary>
        public static string GetClipboardText()
        {
            return Context.PrimaryKeyboard.ClipboardText;
        }

        /// <summary>
        /// Locks the cursor in the Window.
        /// </summary>
        public static void LockCursor()
        {
            Context.PrimaryMouse.Cursor.CursorMode = CursorMode.Hidden;
        }

        /// <summary>
        /// Unlocks the cursor from the Window.
        /// </summary>
        public static void UnlockCursor()
        {
            Context.PrimaryMouse.Cursor.CursorMode = CursorMode.Normal;
        }
    }
}
