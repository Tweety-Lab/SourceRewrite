namespace VistaGUI.Scripting.References
{
    /// <summary>
    /// Generic reference to Image Elements.
    /// </summary>
    public class VistaImage : VistaElement
    {
        /// <summary>
        /// The Source path of the Image.
        /// </summary>
        public string ImageSource
        {
            get => GetProperty("src");
            set => SetProperty("src", value);
        }

        public VistaImage(string ID, VistaScriptingContext context) : base(ID, context) { }
    }
}
