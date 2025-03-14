using SourceRewrite.Maps;
using SourceRewrite.Objects;
using System.Reflection;

namespace SourceRewrite.Components
{
    /// <summary>
    /// Base Component Class.
    /// </summary>
    public class GameComponent
    {
        /// <summary>
        /// Reference to the parent GameObject, This is just a reference, setting it does nothing.
        /// </summary>
        public GameObject GameObject { get; set; }

        /// <summary>
        /// Runs once per frame.
        /// </summary>
        public virtual void Update(float deltaTime) { }

        /// <summary>
        /// Runs once on start.
        /// </summary>
        public virtual void Start() { }

        /// <summary>
        /// Runs once on destroy.
        /// </summary>
        public virtual void OnDestroy() { }

        /// <summary>
        /// Sets a property of the component.
        /// </summary>
        public void SetProperty(string name, object value)
        {
            // Get the type of the class that contains the fields
            Type classType = this.GetType();

            // Get all fields of the game component class
            FieldInfo[] fields = classType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            // Iterate over each field to check for the MapProperty attribute and name
            foreach (var field in fields)
            {
                // Check if the field has the MapProperty attribute
                MapPropertyAttribute attribute = field.GetCustomAttribute<MapPropertyAttribute>();

                if (attribute != null)
                {
                    // Compare the attribute's Name property with the provided input
                    if (attribute.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                    {
                        // Check if the field is of type boolean
                        if (field.FieldType == typeof(bool))
                        {
                            // Convert 0 to false and 1 to true if value is an integer
                            if (value is int intValue)
                            {
                                field.SetValue(this, intValue == 1);
                            }
                            else if (value is bool boolValue)
                            {
                                field.SetValue(this, boolValue);
                            }
                        }
                        else
                        {
                            // Attempt to set the field to the provided value
                            field.SetValue(this, value);
                        }
                    }
                }
            }
        }
    }
}
