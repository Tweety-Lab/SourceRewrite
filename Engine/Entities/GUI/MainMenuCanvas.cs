using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SourceRewrite.Entities.GUI
{
    public class MainMenuCanvas : ScreenspaceGUICanvas
    {
        public override void Start()
        {
            PanelName = "menu/main-menu.html";
            base.Start();
        }
    }
}
