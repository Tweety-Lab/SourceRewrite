using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SourceRewrite.Modding
{
    public static class ModSystem
    {
        // Loaded Assemblies
        public static List<Assembly> LoadedModAssemblies = new List<Assembly>();

        // Method to load external modules
        public static void LoadModules(string modsDirectory)
        {
            if (!Directory.Exists(modsDirectory))
                return;

            // Load all DLL files in the mods directory
            foreach (string dllPath in Directory.GetFiles(modsDirectory, "*.dll"))
            {
                try
                {
                    Assembly assembly = Assembly.LoadFrom(dllPath);
                    LoadedModAssemblies.Add(assembly);
                    Console.WriteLine($"Loaded module: {assembly.GetName().Name}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to load module {dllPath}: {ex.Message}");
                }
            }
        }

        // We load "Game.dll" as a Mod
        public static void LoadGameModule()
        {
            // Load Game DLL
            try
            {
                Assembly assembly = Assembly.LoadFrom("Game.dll");
                LoadedModAssemblies.Add(assembly);
                Console.WriteLine($"Loaded module: {assembly.GetName().Name}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to load Game Module: {ex.Message}");
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
    }
}
