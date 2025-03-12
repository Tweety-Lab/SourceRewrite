namespace VistaGUI
{
    // Update all GUI Views
    public static class VistaContext
    {
        public static List<VistaView> Views = new List<VistaView>();

        // This needs to be ran once per frame somewhere in the program
        public static void Update()
        {
            foreach (VistaView view in Views)
            {
                view.Update();
            }
        }
    }
}
