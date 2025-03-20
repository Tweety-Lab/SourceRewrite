using System.Reflection;

namespace SourceRewrite.Modding
{
    public static class ModSystem
    {
        // Loaded Assemblies
        public static List<Assembly> LoadedModAssemblies = new List<Assembly>();

        // Method to load external modules
        public static void LoadModsFromDir(string modsDirectory)
        {
            if (!Directory.Exists(modsDirectory))
            {
                Console.WriteLine($"Mods directory '{modsDirectory}' does not exist.");
                return;
            }

            // Load all DLL files in the mods directory
            foreach (string dllPath in Directory.GetFiles(modsDirectory, "*.dll"))
            {
                try
                {
                    Assembly assembly = Assembly.LoadFrom(dllPath);
                    LoadedModAssemblies.Add(assembly);
                    Console.WriteLine($"Loaded Mod Assembly: {assembly.GetName().Name}.dll");

                    // Call OnLoad for all IMod implementations in the loaded assembly
                    CallOnLoadForMods(assembly);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to load Mod Assembly {dllPath}: {ex.Message}");
                }
            }
        }

        // Load individual Mod Module
        public static void LoadModAssembly(string name)
        {
            // Load DLL
            try
            {
                Assembly assembly = Assembly.LoadFrom(name);
                LoadedModAssemblies.Add(assembly);
                Console.WriteLine($"Loaded Mod Assembly: {assembly.GetName().Name}.dll");

                // Call OnLoad for all IMod implementations in the loaded assembly
                CallOnLoadForMods(assembly);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to load Mod Assembly: {ex.Message}");
            }
        }

        // Type resolution method
        public static Type FindTypeInLoadedAssemblies(string typeName)
        {
            // Try to find in current assembly first
            Type type = Type.GetType(typeName);
            if (type != null)
                return type;

            // Search in loaded mod assemblies
            foreach (Assembly assembly in LoadedModAssemblies)
            {
                type = assembly.GetType(typeName);
                if (type != null)
                    return type;
            }

            // Search more thoroughly through all loaded assemblies as fallback
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                type = assembly.GetType(typeName);
                if (type != null)
                    return type;
            }

            return null;
        }

        // Calls the OnLoad method for all types in the assembly that implement IMod
        private static void CallOnLoadForMods(Assembly assembly)
        {
            // Get all types in the assembly that implement IMod
            var modTypes = assembly.GetTypes()
                                   .Where(t => typeof(IMod).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
                                   .ToList();

            // Instantiate and call OnLoad on each mod type
            foreach (var modType in modTypes)
            {
                try
                {
                    var modInstance = Activator.CreateInstance(modType) as IMod;
                    modInstance?.OnLoad(); // Invoke OnLoad if the instance is not null
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to invoke OnLoad for mod {modType.Name}: {ex.Message}");
                }
            }
        }

        // Checks if a Mod assembly is loaed from it's name
        public static bool IsModLoaded(string modName)
        {
            return ModSystem.LoadedModAssemblies.Any(a => a.GetName().Name == modName);
        }
    }

    // Mod Entry Point
    public interface IMod
    {
        void OnLoad();
        void OnUnload();
    }
}
