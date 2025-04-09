using SourceRewrite.Attributes;
using SourceRewrite.PhysicsSystem;
using System.Numerics;
using System.Reflection;

namespace SourceRewrite.Entities
{
    public class BaseEntity
    {
        #region HierarchyManagement
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
        public List<BaseEntity> Children { get; } = new List<BaseEntity>();

        #endregion

        // Name of Entity
        public string Name { get; set; }

        // Controls whether Entity logic is enabled
        public bool IsEnabled { get; set; } = true;
        
        // IO Outputs
        public List<EntityIOConnection> Outputs { get; set; } = new List<EntityIOConnection>();

        // Named constructor
        public BaseEntity(string name)
        {
            Name = name;
            Parent = EntityManager.MapContainer; // Auto Set Parent to Map
        }

        // Nameless constructor
        public BaseEntity()
        {
            Name = "Entity";
            Parent = EntityManager.MapContainer; // Auto Set Parent to Map
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

#if EDITOR
        /// <summary>
        /// Runs when Gizmos are drawing for this Entity
        /// </summary>
        public virtual void DrawGizmos() { }

        /// <summary>
        /// Runs when Gizmos are drawing for this Entity and it's selected
        /// </summary>
        public virtual void DrawGizmosSelected() { }
#endif

        /// <summary>
        /// Destroys the Entity.
        /// </summary>
        public void DestroyDeferred()
        {
            // Add the Entity to the destruction queue
            EntityManager.EntitiesToDestroy.Enqueue(this);

            // Run Entity destroy logic
            OnDestroy();

            // Remove any physics
            if (this is PointEntity)
            {
                Physics.DestroyPhysicsEntity(this as PointEntity);
            }

            // Prevent enumeration issues
            List<BaseEntity> childrenToDestroy = new List<BaseEntity>(Children);

            // Recursively destroy all child objects
            foreach (var child in childrenToDestroy)
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
        /// Set a Entity Property to a value.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void SetProperty(string name, object value)
        {
            // Find the property or field that has the EntityProperty attribute matching the name
            var propertyOrField = this.GetType()
                                      .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                      .FirstOrDefault(p => p.GetCustomAttribute<EntityPropertyAttribute>()?.Name == name);

            if (propertyOrField != null && propertyOrField.CanWrite)
            {
                propertyOrField.SetValue(this, Convert.ChangeType(value, propertyOrField.PropertyType));
                return;
            }

            var field = this.GetType()
                            .GetFields(BindingFlags.Public | BindingFlags.Instance)
                            .FirstOrDefault(f => f.GetCustomAttribute<EntityPropertyAttribute>()?.Name == name);

            if (field != null)
            {
                field.SetValue(this, Convert.ChangeType(value, field.FieldType));
            }
        }

        /// <summary>
        /// Fire an Entity Output.
        /// </summary>
        /// <param name="outputName"></param>
        public void FireOutput(string outputName)
        {
            // Get all outputs with the matching name
            var matchingOutputs = Outputs.Where(o => o.OutputName == outputName);

            // Fire each matching output
            foreach (var output in matchingOutputs)
            {
                output.Fire();
            }
        }
    }
}
