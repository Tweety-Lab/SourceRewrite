using SourceRewrite.Entities.GUI;
using SourceRewrite.InputSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SourceRewrite.Entities.ConsoleCanvas
{
    public class ConsoleCanvas : ScreenspaceGUICanvas
    {
        public override void Start()
        {
            PanelName = "console/console.html";
            base.Start();

            Canvas.Visible = false;


            Input.KeyDownEvent += (keyboard, key, _) =>
            {
                if (key == Silk.NET.Input.Key.GraveAccent)
                {
                    // Print the key that was pressed
                    Console.WriteLine($"Key pressed: {key}");

                    Canvas.Visible = !Canvas.Visible;
                }
            };
        }
    }
}
