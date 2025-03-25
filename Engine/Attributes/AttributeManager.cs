using System.Reflection;

namespace SourceRewrite.Attributes
{
    /// <summary>
    /// Static utility class for managing and retrieving members with specific attributes
    /// </summary>
    public static class AttributeManager
    {
        // Cache for types with specific attributes
        private static readonly Dictionary<Type, List<Type>> TypeCache = new Dictionary<Type, List<Type>>();

        // Cache for fields with specific attributes
        private static readonly Dictionary<Type, Dictionary<FieldInfo, Attribute>> FieldCache = new Dictionary<Type, Dictionary<FieldInfo, Attribute>>();

        // Cache for properties with specific attributes
        private static readonly Dictionary<Type, Dictionary<PropertyInfo, Attribute>> PropertyCache = new Dictionary<Type, Dictionary<PropertyInfo, Attribute>>();

        // Cache for methods with specific attributes
        private static readonly Dictionary<Type, Dictionary<MethodInfo, Attribute>> MethodCache = new Dictionary<Type, Dictionary<MethodInfo, Attribute>>();

        // Cache for attribute values (used for quick lookups)
        private static readonly Dictionary<string, MemberInfo> AttributeValueCache = new Dictionary<string, MemberInfo>();

        /// <summary>
        /// Initializes the attribute caches for a specific attribute type
        /// </summary>
        /// <typeparam name="T">The attribute type to cache</typeparam>
        public static void Initialize<T>() where T : Attribute
        {
            Type attributeType = typeof(T);

            // Initialize type cache
            if (!TypeCache.ContainsKey(attributeType))
            {
                TypeCache[attributeType] = new List<Type>();
                CacheTypesWithAttribute<T>(TypeCache[attributeType]);
            }

            // Initialize field cache
            if (!FieldCache.ContainsKey(attributeType))
            {
                FieldCache[attributeType] = new Dictionary<FieldInfo, Attribute>();
                CacheFieldsWithAttribute<T>(FieldCache[attributeType]);
            }

            // Initialize property cache
            if (!PropertyCache.ContainsKey(attributeType))
            {
                PropertyCache[attributeType] = new Dictionary<PropertyInfo, Attribute>();
                CachePropertiesWithAttribute<T>(PropertyCache[attributeType]);
            }

            // Initialize method cache
            if (!MethodCache.ContainsKey(attributeType))
            {
                MethodCache[attributeType] = new Dictionary<MethodInfo, Attribute>();
                CacheMethodsWithAttribute<T>(MethodCache[attributeType]);
            }
        }

        /// <summary>
        /// Gets all classes with the specified attribute
        /// </summary>
        /// <typeparam name="T">The attribute type to search for</typeparam>
        /// <returns>List of types with the attribute</returns>
        public static List<Type> GetTypesWithAttribute<T>() where T : Attribute
        {
            Type attributeType = typeof(T);

            if (!TypeCache.ContainsKey(attributeType))
            {
                Initialize<T>();
            }

            return TypeCache[attributeType];
        }

        /// <summary>
        /// Gets all fields with the specified attribute
        /// </summary>
        /// <typeparam name="T">The attribute type to search for</typeparam>
        /// <returns>Dictionary of fields and their associated attributes</returns>
        public static Dictionary<FieldInfo, Attribute> GetFieldsWithAttribute<T>() where T : Attribute
        {
            Type attributeType = typeof(T);

            if (!FieldCache.ContainsKey(attributeType))
            {
                Initialize<T>();
            }

            return FieldCache[attributeType];
        }

        /// <summary>
        /// Gets all properties with the specified attribute
        /// </summary>
        /// <typeparam name="T">The attribute type to search for</typeparam>
        /// <returns>Dictionary of properties and their associated attributes</returns>
        public static Dictionary<PropertyInfo, Attribute> GetPropertiesWithAttribute<T>() where T : Attribute
        {
            Type attributeType = typeof(T);

            if (!PropertyCache.ContainsKey(attributeType))
            {
                Initialize<T>();
            }

            return PropertyCache[attributeType];
        }

        /// <summary>
        /// Gets all methods with the specified attribute
        /// </summary>
        /// <typeparam name="T">The attribute type to search for</typeparam>
        /// <returns>Dictionary of methods and their associated attributes</returns>
        public static Dictionary<MethodInfo, Attribute> GetMethodsWithAttribute<T>() where T : Attribute
        {
            Type attributeType = typeof(T);

            if (!MethodCache.ContainsKey(attributeType))
            {
                Initialize<T>();
            }

            return MethodCache[attributeType];
        }

        /// <summary>
        /// Gets a field by the value of its attribute property
        /// </summary>
        /// <typeparam name="T">The attribute type</typeparam>
        /// <typeparam name="TValue">The property value type</typeparam>
        /// <param name="propertyName">The name of the property on the attribute</param>
        /// <param name="value">The value to search for</param>
        /// <returns>The field with the matching attribute property value</returns>
        public static FieldInfo GetFieldByAttributeValue<T, TValue>(string propertyName, TValue value) where T : Attribute
        {
            // Create lookup key
            string key = $"{typeof(T).FullName}_{propertyName}_{value}";

            // Check cache first
            if (AttributeValueCache.ContainsKey(key) && AttributeValueCache[key] is FieldInfo field)
            {
                return field;
            }

            var fields = GetFieldsWithAttribute<T>();
            foreach (var pair in fields)
            {
                var attribute = pair.Value;
                var property = attribute.GetType().GetProperty(propertyName);

                if (property != null && property.GetValue(attribute).Equals(value))
                {
                    AttributeValueCache[key] = pair.Key;
                    return pair.Key;
                }
            }

            return null;
        }

        /// <summary>
        /// Gets a property by the value of its attribute property
        /// </summary>
        /// <typeparam name="T">The attribute type</typeparam>
        /// <typeparam name="TValue">The property value type</typeparam>
        /// <param name="propertyName">The name of the property on the attribute</param>
        /// <param name="value">The value to search for</param>
        /// <returns>The property with the matching attribute property value</returns>
        public static PropertyInfo GetPropertyByAttributeValue<T, TValue>(string propertyName, TValue value) where T : Attribute
        {
            // Create lookup key
            string key = $"{typeof(T).FullName}_{propertyName}_{value}";

            // Check cache first
            if (AttributeValueCache.ContainsKey(key) && AttributeValueCache[key] is PropertyInfo property)
            {
                return property;
            }

            var properties = GetPropertiesWithAttribute<T>();
            foreach (var pair in properties)
            {
                var attribute = pair.Value;
                var attrProperty = attribute.GetType().GetProperty(propertyName);

                if (attrProperty != null && attrProperty.GetValue(attribute).Equals(value))
                {
                    AttributeValueCache[key] = pair.Key;
                    return pair.Key;
                }
            }

            return null;
        }

        /// <summary>
        /// Gets a method by the value of its attribute property
        /// </summary>
        /// <typeparam name="T">The attribute type</typeparam>
        /// <typeparam name="TValue">The property value type</typeparam>
        /// <param name="propertyName">The name of the property on the attribute</param>
        /// <param name="value">The value to search for</param>
        /// <returns>The method with the matching attribute property value</returns>
        public static MethodInfo GetMethodByAttributeValue<T, TValue>(string propertyName, TValue value) where T : Attribute
        {
            // Create lookup key
            string key = $"{typeof(T).FullName}_{propertyName}_{value}";

            // Check cache first
            if (AttributeValueCache.ContainsKey(key) && AttributeValueCache[key] is MethodInfo method)
            {
                return method;
            }

            var methods = GetMethodsWithAttribute<T>();
            foreach (var pair in methods)
            {
                var attribute = pair.Value;
                var property = attribute.GetType().GetProperty(propertyName);

                if (property != null && property.GetValue(attribute).Equals(value))
                {
                    AttributeValueCache[key] = pair.Key;
                    return pair.Key;
                }
            }

            return null;
        }

        /// <summary>
        /// Gets a type by the value of its attribute property
        /// </summary>
        /// <typeparam name="T">The attribute type</typeparam>
        /// <typeparam name="TValue">The property value type</typeparam>
        /// <param name="propertyName">The name of the property on the attribute</param>
        /// <param name="value">The value to search for</param>
        /// <returns>The type with the matching attribute property value</returns>
        public static Type GetTypeByAttributeValue<T, TValue>(string propertyName, TValue value) where T : Attribute
        {
            // Create lookup key
            string key = $"{typeof(T).FullName}_{propertyName}_{value}";

            // Check cache first
            if (AttributeValueCache.ContainsKey(key) && AttributeValueCache[key] is Type type)
            {
                return type;
            }

            var types = GetTypesWithAttribute<T>();
            foreach (var thisType in types)
            {
                var attribute = thisType.GetCustomAttribute<T>();
                var property = attribute.GetType().GetProperty(propertyName);

                if (property != null && property.GetValue(attribute).Equals(value))
                {
                    AttributeValueCache[key] = thisType;
                    return thisType;
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the attribute of a specific type from a field
        /// </summary>
        /// <typeparam name="T">The attribute type</typeparam>
        /// <param name="field">The field to get the attribute from</param>
        /// <returns>The attribute instance</returns>
        public static T GetAttribute<T>(FieldInfo field) where T : Attribute
        {
            return (T)field.GetCustomAttribute(typeof(T));
        }

        /// <summary>
        /// Gets the attribute of a specific type from a property
        /// </summary>
        /// <typeparam name="T">The attribute type</typeparam>
        /// <param name="property">The property to get the attribute from</param>
        /// <returns>The attribute instance</returns>
        public static T GetAttribute<T>(PropertyInfo property) where T : Attribute
        {
            return (T)property.GetCustomAttribute(typeof(T));
        }

        /// <summary>
        /// Gets the attribute of a specific type from a method
        /// </summary>
        /// <typeparam name="T">The attribute type</typeparam>
        /// <param name="method">The method to get the attribute from</param>
        /// <returns>The attribute instance</returns>
        public static T GetAttribute<T>(MethodInfo method) where T : Attribute
        {
            return (T)method.GetCustomAttribute(typeof(T));
        }

        /// <summary>
        /// Gets the attribute of a specific type from a type
        /// </summary>
        /// <typeparam name="T">The attribute type</typeparam>
        /// <param name="type">The type to get the attribute from</param>
        /// <returns>The attribute instance</returns>
        public static T GetAttribute<T>(Type type) where T : Attribute
        {
            return (T)type.GetCustomAttribute(typeof(T));
        }

        /// <summary>
        /// Cache all types with a specific attribute
        /// </summary>
        private static void CacheTypesWithAttribute<T>(List<Type> list) where T : Attribute
        {
            // Get types from the executing assembly
            list.AddRange(Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t => t.IsDefined(typeof(T), false)));

            // Get types from mod assemblies
            foreach (var assembly in Modding.ModSystem.LoadedModAssemblies)
            {
                list.AddRange(assembly.GetTypes()
                    .Where(t => t.IsDefined(typeof(T), false)));
            }
        }

        /// <summary>
        /// Cache all fields with a specific attribute
        /// </summary>
        private static void CacheFieldsWithAttribute<T>(Dictionary<FieldInfo, Attribute> dict) where T : Attribute
        {
            // Process fields from the executing assembly
            ProcessFieldsFromAssembly<T>(Assembly.GetExecutingAssembly(), dict);

            // Process fields from mod assemblies
            foreach (var assembly in Modding.ModSystem.LoadedModAssemblies)
            {
                ProcessFieldsFromAssembly<T>(assembly, dict);
            }
        }

        /// <summary>
        /// Cache all properties with a specific attribute
        /// </summary>
        private static void CachePropertiesWithAttribute<T>(Dictionary<PropertyInfo, Attribute> dict) where T : Attribute
        {
            // Process properties from the executing assembly
            ProcessPropertiesFromAssembly<T>(Assembly.GetExecutingAssembly(), dict);

            // Process properties from mod assemblies
            foreach (var assembly in Modding.ModSystem.LoadedModAssemblies)
            {
                ProcessPropertiesFromAssembly<T>(assembly, dict);
            }
        }

        /// <summary>
        /// Cache all methods with a specific attribute
        /// </summary>
        private static void CacheMethodsWithAttribute<T>(Dictionary<MethodInfo, Attribute> dict) where T : Attribute
        {
            // Process methods from the executing assembly
            ProcessMethodsFromAssembly<T>(Assembly.GetExecutingAssembly(), dict);

            // Process methods from mod assemblies
            foreach (var assembly in Modding.ModSystem.LoadedModAssemblies)
            {
                ProcessMethodsFromAssembly<T>(assembly, dict);
            }
        }

        /// <summary>
        /// Process fields with attribute from a specific assembly
        /// </summary>
        private static void ProcessFieldsFromAssembly<T>(Assembly assembly, Dictionary<FieldInfo, Attribute> dict) where T : Attribute
        {
            foreach (var type in assembly.GetTypes())
            {
                foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static))
                {
                    var attribute = field.GetCustomAttribute<T>();
                    if (attribute != null)
                    {
                        dict[field] = attribute;
                    }
                }
            }
        }

        /// <summary>
        /// Process properties with attribute from a specific assembly
        /// </summary>
        private static void ProcessPropertiesFromAssembly<T>(Assembly assembly, Dictionary<PropertyInfo, Attribute> dict) where T : Attribute
        {
            foreach (var type in assembly.GetTypes())
            {
                foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static))
                {
                    var attribute = property.GetCustomAttribute<T>();
                    if (attribute != null)
                    {
                        dict[property] = attribute;
                    }
                }
            }
        }

        /// <summary>
        /// Process methods with attribute from a specific assembly
        /// </summary>
        private static void ProcessMethodsFromAssembly<T>(Assembly assembly, Dictionary<MethodInfo, Attribute> dict) where T : Attribute
        {
            foreach (var type in assembly.GetTypes())
            {
                foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static))
                {
                    var attribute = method.GetCustomAttribute<T>();
                    if (attribute != null)
                    {
                        dict[method] = attribute;
                    }
                }
            }
        }

        /// <summary>
        /// Clears all cached attribute data
        /// </summary>
        public static void ClearCache()
        {
            TypeCache.Clear();
            FieldCache.Clear();
            PropertyCache.Clear();
            MethodCache.Clear();
            AttributeValueCache.Clear();
        }
    }
}