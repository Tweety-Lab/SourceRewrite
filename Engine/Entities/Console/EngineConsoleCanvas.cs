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
                Msg("Submitted Console Command: " + args[0]);
            });
        }

        // Log a message to the console
        public void Msg(string message)
        {
            VistaElement logArea = Canvas.GetElement("log-area");
            logArea.InnerHTML = $"{logArea.InnerHTML}<br>{message}";
        }

        // Log a yellow warning message to the console
        public void Warn(string message)
        {
            VistaElement logArea = Canvas.GetElement("log-area");
            logArea.InnerHTML = $"{logArea.InnerHTML}<br><span style='color: yellow;'>{message}</span>";
        }

        // Log a red error message to the console
        public void Error(string message)
        {
            VistaElement logArea = Canvas.GetElement("log-area");
            logArea.InnerHTML = $"{logArea.InnerHTML}<br><span style='color: red;'>{message}</span>";
        }
    }
}
