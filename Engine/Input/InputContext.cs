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

        // Action States
        public Dictionary<string, bool> ActionStates = new();      // Current frame
        public Dictionary<string, bool> PreviousActionStates = new(); // Previous frame

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
            // Update mouse delta
            MouseDelta = PrimaryMouse.Position - lastMousePos;
            lastMousePos = PrimaryMouse.Position;

            // Copy current action states to previous
            foreach (var kvp in ActionStates)
            {
                PreviousActionStates[kvp.Key] = kvp.Value;
            }

            // Update current action states
            foreach (var action in Keybinds.Keys)
            {
                ActionStates[action] = Input.GetDown(action);
            }
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
}
