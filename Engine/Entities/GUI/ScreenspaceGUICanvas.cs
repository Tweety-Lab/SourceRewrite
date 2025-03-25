using SourceRewrite.Attributes;

namespace SourceRewrite.Entities.GUI
{
    /// <summary>
    /// Abstraction for GUICanvasEntity that renders to ScreenSpace
    /// </summary>
    public class ScreenspaceGUICanvas : BaseEntity
    {
        [EntityProperty("panelName")]
        public string PanelName { get; set; }

        public GUICanvasEntity Canvas;

        public override void Start()
        {
            Canvas = new GUICanvasEntity();
            Canvas.IsTransparent = true;
            Canvas.PanelName = PanelName;
            EntityManager.AddGlobalEntity(Canvas);

            // Manually start the canvas component
            Canvas.Start();
        }

        public override void OnDestroy()
        {
            Canvas.DestroyDeferred();
        }

        // Register a GUI Event
        public void RegisterEvent(string name, Action<string[]> action) => Canvas.RegisterEvent(name, action);
        public void RegisterEvent(string name, Action action) => Canvas.RegisterEvent(name, action);

        // Cleanup a GUI Event
        public void UnregisterEvent(string name) => Canvas.UnregisterEvent(name);
    }
}
