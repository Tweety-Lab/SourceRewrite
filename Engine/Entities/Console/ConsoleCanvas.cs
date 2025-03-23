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
                // On ` key
                if (key == Silk.NET.Input.Key.GraveAccent)
                {
                    // Toggle Visibility
                    Canvas.Visible = !Canvas.Visible;
                }
            };
        }
    }
}
