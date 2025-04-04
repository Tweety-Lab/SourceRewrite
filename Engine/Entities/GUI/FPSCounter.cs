using SourceRewrite.Attributes;
using SourceRewrite.Maps;
using SourceRewrite.TimeSystem;
using VistaGUI.Scripting.References;

namespace SourceRewrite.Entities.GUI.BuiltIn
{
    public class FPSCounter : ScreenspaceGUICanvas
    {
        private static FPSCounter _fpsCounterInstance;

        [ConVar("cl_showfps")]
        public static bool ShowFPS
        {
            get => _fpsCounterInstance != null; // Return true if instance exists
            set
            {
                if (value && _fpsCounterInstance == null)
                {
                    // Create the instance of FPSCounter
                    _fpsCounterInstance = new FPSCounter();
                    _fpsCounterInstance.Parent = EntityManager.MapContainer;
                    _fpsCounterInstance.Start();
                }
                else if (!value && _fpsCounterInstance != null)
                {
                    // Destroy the FPSCounter
                    _fpsCounterInstance.DestroyDeferred();
                    _fpsCounterInstance = null;
                }
            }
        }


        public override void Start()
        {
            PanelName = "utilities/fpscounter/counter.html";
            base.Start();
        }

        public override void Update()
        {
            // Update the FPS counter
            int fps = (int)(1.0f / Time.DeltaTime);
            Canvas.GetElementAsType<VistaText>("fps-counter").TextContent = $"{fps} fps on {MapSystem.CurrentMap.BSPFilePath.Replace("../..//", "")}";
        }
    }
}
