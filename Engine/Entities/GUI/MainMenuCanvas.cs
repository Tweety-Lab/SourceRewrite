using SourceRewrite.Attributes;

namespace SourceRewrite.Entities.GUI
{
    [Entity("main_menu_canvas")]
    public class MainMenuCanvas : ScreenspaceGUICanvas
    {
        public override void Start()
        {
            PanelName = "menu/main-menu.html";
            base.Start();
        }
    }
}
