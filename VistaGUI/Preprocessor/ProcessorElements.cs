namespace VistaGUI.Preprocessor
{
    public static class ProcessorElements
    {
        /// <summary>
        /// Maps Element Names to File Paths for their scripts.
        /// </summary>
        public static Dictionary<string, string> ElementMap = new Dictionary<string, string>()
        {
            // ELEMENT NAME, FILE PATH

            // Panels
            { "vista-panel", "elements/panels.js" },
        };
    }
}
