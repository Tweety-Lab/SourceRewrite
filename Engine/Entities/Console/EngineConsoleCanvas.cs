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
            EngineConsole.ConsoleCanvas = this;

            PanelName = "console/console.html";
            base.Start();

            Input.KeyDownEvent += (keyboard, key, _) =>
            {
                // On ` key
                if (key == Silk.NET.Input.Key.GraveAccent)
                {
                }
            };

            RegisterEvent("SubmitCommand", (args) =>
            {
                Msg("] " + args[0]); // Display the typed command
                string result = EngineConsole.EvaluateCommand(args[0]); // Run the typed command and get the result

                // Only log the result if it was abnormal (not empty)
                if (!string.IsNullOrEmpty(result))
                {
                    Msg(result);
                }
            });
        }

        // Log a message to the console
        public void Msg(string message)
        {
            VistaElement logArea = Canvas.GetElement("log-area");

            // Only append <br> if the logArea is not empty
            if (!string.IsNullOrEmpty(logArea.InnerHTML))
            {
                logArea.InnerHTML = $"{logArea.InnerHTML}<br>{message}";
            }
            else
            {
                logArea.InnerHTML = message;
            }
        }

        // Log a yellow warning message to the console
        public void Warning(string message)
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
