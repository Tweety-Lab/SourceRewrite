using Silk.NET.GLFW;
using Silk.NET.Input;
using SourceRewrite.Windowing;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

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

        /// <summary>
        /// Returns True if the chosen key is pressed down.
        /// </summary>
        public static bool GetKeyDown(Key key)
        {
            return GameWindow.CurrentWindow.Input.PrimaryKeyboard.IsKeyPressed(key);
        }

        /// <summary>
        /// Returns True if chosen Mouse Button is down.
        /// </summary>
        public static bool GetMouseButtonDown(int mouseButton)
        {
            // Convert the int to a Silk.NET MouseButton
            Silk.NET.Input.MouseButton button = (Silk.NET.Input.MouseButton)mouseButton;

            // Check if the specific mouse button is pressed
            return GameWindow.CurrentWindow.Input.PrimaryMouse.IsButtonPressed(button);
        }

        /// <summary>
        /// Returns current Mouse Position as Vector2.
        /// </summary>
        public static Vector2 GetMousePosition()
        {
            return GameWindow.CurrentWindow.Input.PrimaryMouse.Position;
        }

        /// <summary>
        /// Returns current Mouse X Position.
        /// </summary>
        public static float GetMouseX()
        {
            return GameWindow.CurrentWindow.Input.PrimaryMouse.Position.X;
        }

        /// <summary>
        /// Returns current Mouse Y Position.
        /// </summary>
        public static float GetMouseY()
        {
            return GameWindow.CurrentWindow.Input.PrimaryMouse.Position.Y;
        }

        /// <summary>
        /// Returns Mouse Movement since last Frame.
        /// </summary>
        public static Vector2 GetMouseMovement()
        {
            return GameWindow.CurrentWindow.Input.MouseDelta;
        }

        /// <summary>
        /// Returns X Mouse Movement since last Frame.
        /// </summary>
        public static float GetMouseXMovement()
        {
            return GameWindow.CurrentWindow.Input.MouseDelta.X;
        }

        /// <summary>
        /// Returns Y Mouse Movement since last Frame.
        /// </summary>
        public static float GetMouseYMovement()
        {
            return GameWindow.CurrentWindow.Input.MouseDelta.Y;
        }

        /// <summary>
        /// Locks the cursor in the Window.
        /// </summary>
        public static void LockCursor()
        {
            GameWindow.CurrentWindow.Input.PrimaryMouse.Cursor.CursorMode = CursorMode.Hidden;
        }

        /// <summary>
        /// Unlocks the cursor from the Window.
        /// </summary>
        public static void UnlockCursor()
        {
            GameWindow.CurrentWindow.Input.PrimaryMouse.Cursor.CursorMode = CursorMode.Normal;
        }
    }
}
