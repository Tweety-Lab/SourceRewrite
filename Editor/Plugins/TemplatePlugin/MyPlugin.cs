using SourceRewrite.Modding;

namespace TemplatePlugin
{
    public class MyPlugin : IMod
    {
        // Runs when the mod is loaded
        public void OnLoad()
        {
            Console.WriteLine("Loaded MyPlugin!");
        }

        // Runs when the mod is unloaded
        public void  OnUnload()
        {
            Console.WriteLine("Shutting Down MyPlugin...");
        }
    }
}
