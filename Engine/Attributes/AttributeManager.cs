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
            // Get all types with the AlwaysExecute attribute
            List<Type> types = Assembly.GetExecutingAssembly()
                                        .GetTypes()
                                        .Where(t => t.IsDefined(typeof(AlwaysExecuteAttribute), false))
                                        .ToList();

            AlwaysExecuteEntitiesTypes = types;
        }
    }
}
