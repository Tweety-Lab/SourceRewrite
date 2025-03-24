using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SourceRewrite.Attributes
{
    /// <summary>
    /// Helps with caching classes with certain attributes
    /// </summary>
    public static class AttributeCache
    {
        // All Entities
        public static List<Type> AlwaysExecuteEntitiesTypes = new List<Type>();

        static AttributeCache()
        {
            // Add all types with attributes to the list
            AddTypesWithAttribute<AlwaysExecuteAttribute>(AlwaysExecuteEntitiesTypes);
        }

        private static void AddTypesWithAttribute<T>(List<Type> list)
        {
            // Get all types with the attribute from the executing assembly
            List<Type> types = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t => t.IsDefined(typeof(T), false))
                .ToList();
            // Add to list
            list.AddRange(types);

            // Also search through all loaded mod assemblies
            foreach (var assembly in SourceRewrite.Modding.ModSystem.LoadedModAssemblies)
            {
                // Get types with the attribute from mod assemblies
                types = assembly.GetTypes()
                    .Where(t => t.IsDefined(typeof(T), false))
                    .ToList();
                // Add to list
                list.AddRange(types);
            }
        }
    }
}
