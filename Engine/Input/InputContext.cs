using Silk.NET.GLFW;
using Silk.NET.Input;
using SourceRewrite.Windowing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SourceRewrite.Input
{
    public class InputContext
    {
        public IKeyboard PrimaryKeyboard;
        public IMouse PrimaryMouse;
        public InputContext(IInputContext input)
        {
            PrimaryKeyboard = input.Keyboards.FirstOrDefault();
            PrimaryMouse = input.Mice.FirstOrDefault();
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
    }
}
