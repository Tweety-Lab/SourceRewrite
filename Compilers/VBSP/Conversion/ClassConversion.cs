namespace VBSP.Conversion
{
    public static class ClassConversion
    {
        // Create dictionary to store Valve to Our types
        public static Dictionary<string, string> ClassMap = new Dictionary<string, string>()
        {
            // Valve Class Name | Our Class Name
            { "light",          "SourceRewrite_Entities_PointLight" },
            { "prop_static",    "SourceRewrite_Entities_PropEntity" },
            { "info_player_start", "Game_Entities_CameraController" },
            { "func_instance", "SourceRewrite_Entities_Prefab" },
            { "vgui_screen", "SourceRewrite_Entities_GUICanvas" }
        };
    }
}
