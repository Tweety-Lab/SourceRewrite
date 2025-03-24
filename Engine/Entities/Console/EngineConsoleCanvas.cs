using SourceRewrite.Entities.GUI;
using SourceRewrite.InputSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VistaGUI.Scripting.References;

namespace SourceRewrite
{
    public class EngineConsoleCanvas : ScreenspaceGUICanvas
    {
        public override void Start()
        {
            PanelName = "console/console.html";
            base.Start();

            Input.KeyDownEvent += (keyboard, key, _) =>
            {
                // On ` key
                if (key == Silk.NET.Input.Key.GraveAccent)
                {
                }
            };

            string currentLog = String.Empty;
            RegisterEvent("SubmitCommand", (args) =>
            {
                Log("Submitted Console Command: " + args[0]);
            });
        }

        public void Log(string message)
        {
            VistaElement logArea = Canvas.GetElement("log-area");
            logArea.SetProperty("value", $"{logArea.GetProperty("value")}\n{message}");
        }
    }
}
