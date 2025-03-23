using SourceRewrite.Entities.GUI;
using SourceRewrite.InputSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VistaGUI.Scripting.References;

namespace SourceRewrite.Entities.ConsoleCanvas
{
    public class ConsoleCanvas : ScreenspaceGUICanvas
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
                Console.WriteLine("Submitted Console Command: " + args[0]);

                // Add the command to the log
                VistaElement logArea = Canvas.GetElement("log-area");
                currentLog += $"] {args[0]}\n";
                logArea.SetProperty("value", currentLog);
            });
        }
    }
}
