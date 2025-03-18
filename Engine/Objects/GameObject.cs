using SourceRewrite.Components;
using SourceRewrite.Maps;

namespace SourceRewrite.Objects
{
    public class GameObject
    {
        // Every attached Component
        public List<GameComponent> Components { get; private set; } = new List<GameComponent>();

        /// <summary>
        /// Children GameObjects.
        /// </summary>
        public List<GameObject> Children { get; private set; } = new List<GameObject>();

        // Name of GameObject
        public string Name { get; set; }

        // Every Object needs a Transform
        public Transform Transform { get; set; } = new Transform();

        private GameObject _parent; // backing field to store the parent

        /// <summary>
        /// Parent GameObject.
        /// </summary>
        public GameObject Parent
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

        public GameObject(string name = "GameObject")
        {
            Name = name;
            AddComponent(Transform); // Add Transform component to the components list
        }

        /// <summary>
        /// Runs all GameObject initialization logic.
        /// </summary>
        public void GameObjectStart()
        {
            // Create a copy of the Components list to avoid modification issues during iteration
            List<GameComponent> componentsToStart = new List<GameComponent>(Components);

            // Loop through the copy of components and start them
            foreach (GameComponent comp in componentsToStart)
            {
                comp.Start(); // Call Start on each component to initialize them
            }

            // Recursively loop through the copy of children and start them
            List<GameObject> childrenCopy = new List<GameObject>(Children);

            // Recursively start all child objects
            foreach (GameObject child in childrenCopy)
            {
                child.GameObjectStart();  // Call Start on each child to initialize them
            }
        }

        /// <summary>
        /// Returns the first found Component of specified type.
        /// </summary>
        public ComponentType GetComponentFromType<ComponentType>() where ComponentType : GameComponent
        {
            // Loop through components until we get specified type
            foreach (GameComponent component in Components)
            {
                if(component is ComponentType)
                {
                    // Cast to the found type
                    return (ComponentType) component;
                }
            }
            // Didn't find any of the specified type, return null
            return null;
        }

        /// <summary>
        /// Adds a Component to the GameObject.
        /// </summary>
        public void AddComponent<T>(T component) where T : GameComponent
        {
            Components.Add(component);
            component.GameObject = this;
        }

        /// <summary>
        /// Removes a Component of the specified type from the GameObject.
        /// </summary>
        public void RemoveComponentOfType<ComponentType>() where ComponentType : GameComponent
        {
            // Find and remove the component of the specified type
            var componentToRemove = Components.FirstOrDefault(comp => comp is ComponentType);
            if (componentToRemove != null)
            {
                componentToRemove.OnDestroy(); // Call OnDestroy before removal
                Components.Remove(componentToRemove);
            }
        }


        /// <summary>
        /// Destroys the GameObject.
        /// </summary>
        public void DestroyDeferred()
        {
            // Add the GameObject to the destruction queue
            GameObjectManager.ObjectsToDestroy.Enqueue(this);

            // Recursively destroy all child objects
            foreach (var child in Children)
            {
                child.DestroyDeferred();
            }
        }

        /// <summary>
        /// Find a child GameObject by name
        /// </summary>
        public GameObject FindChild(string name)
        {
            return Children.FirstOrDefault(child => child.Name == name);
        }

        /// <summary>
        /// Find a child GameObject by name recursively through the hierarchy
        /// </summary>
        public GameObject FindInChildren(string name)
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
    }
}
