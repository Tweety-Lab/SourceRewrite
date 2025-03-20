using SourceRewrite.Maps;
using System.Numerics;
using System.Reflection;

namespace SourceRewrite.Entities
{
    public class BaseEntity
    {
        // Entity Transform
        public Transform Transform { get; set; } = new Transform();

        #region Hierarchy
        private BaseEntity _parent; // backing field to store the parent

        /// <summary>
        /// Parent Entity.
        /// </summary>
        public BaseEntity Parent
        {
            get => _parent;
            set
            {
                // Remove from old parent if exists
                if (_parent != null && _parent.Children.Contains(this))
                {
                    _parent.Children.Remove(this);
                }

                _parent = value;

                // Add to new parent if not null
                if (_parent != null && !_parent.Children.Contains(this))
                {
                    _parent.Children.Add(this);
                }
            }
        }

        /// <summary>
        /// Children Entities.
        /// </summary>
        public List<BaseEntity> Children { get; private set; } = new List<BaseEntity>();

        #endregion

        // Name of Entity
        public string Name { get; set; }

        // Named constructor
        public BaseEntity(string name)
        {
            Name = name;
        }

        // Nameless constructor
        public BaseEntity()
        {
            Name = "Entity";
        }


        /// <summary>
        /// Runs all Entity initialization logic.
        /// </summary>
        public virtual void Start() { }

        /// <summary>
        /// All Entity logic.
        /// </summary>
        public virtual void Update() { }

        /// <summary>
        /// Runs once on destruction.
        /// </summary>
        public virtual void OnDestroy() { }

        /// <summary>
        /// Destroys the Entity.
        /// </summary>
        public void DestroyDeferred()
        {
            // Add the Entity to the destruction queue
            EntityManager.EntitiesToDestroy.Enqueue(this);

            // Recursively destroy all child objects
            foreach (var child in Children)
            {
                child.DestroyDeferred();
            }
        }

        /// <summary>
        /// Find a child Entity by name
        /// </summary>
        public BaseEntity GetChild(string name)
        {
            return Children.FirstOrDefault(child => child.Name == name);
        }

        /// <summary>
        /// Find a child Entity by name recursively through the hierarchy
        /// </summary>
        public BaseEntity FindInChildren(string name)
        {
            foreach (var child in Children)
            {
                if (child.Name == name)
                    return child;

                var result = child.FindInChildren(name);
                if (result != null)
                    return result;
            }

            return null;
        }

        /// <summary>
        /// Set a map property to a value.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void SetProperty(string name, object value)
        {
            // Find the property or field that has the MapProperty attribute matching the name
            var propertyOrField = this.GetType()
                                      .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                      .FirstOrDefault(p => p.GetCustomAttribute<MapPropertyAttribute>()?.Name == name);

            if (propertyOrField != null && propertyOrField.CanWrite)
            {
                propertyOrField.SetValue(this, Convert.ChangeType(value, propertyOrField.PropertyType));
                return;
            }

            var field = this.GetType()
                            .GetFields(BindingFlags.Public | BindingFlags.Instance)
                            .FirstOrDefault(f => f.GetCustomAttribute<MapPropertyAttribute>()?.Name == name);

            if (field != null)
            {
                field.SetValue(this, Convert.ChangeType(value, field.FieldType));
            }
        }
    }
}
