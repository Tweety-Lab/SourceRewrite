namespace VBSP.Conversion
{
    public static class ClassConversion
    {
        // Create dictionary to store Valve to Our types
        public static Dictionary<string, string> ClassMap = new Dictionary<string, string>()
        {
            // Valve Class Name | Our Class Name
            { "light",          "SourceRewrite_Components_PointLight" },
            { "prop_static",    "SourceRewrite_Components_MeshRenderer" }
        };
    }
}
