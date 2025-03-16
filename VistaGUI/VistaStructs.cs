namespace VistaGUI
{
    // Vista Config
    public struct GUIConfig
    {
        public bool IsTransparent { get; set; }
        public bool EnableJavaScript { get; set; }

        public string ResourcesPath { get; set; }
    }

    // Vista Key Modifiers
    public enum VistaKeyModifiers
    {
        None = -1,
        AltKey,
        CtrlKey,
        OSKey,
        ShiftKey
    }
}
