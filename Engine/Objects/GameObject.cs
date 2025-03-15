using SourceRewrite.Components;

namespace SourceRewrite.Objects
{
    public class GameObject
    {
        // Every object that exists
        public static List<GameObject> ActiveObjects { get; private set; } = new List<GameObject>();

        // Queue for deferred destruction of GameObjects
        private static Queue<GameObject> _objectsToDestroy = new Queue<GameObject>();

        // Every attached Component
        public List<GameComponent> Components { get; private set; } = new List<GameComponent>();

        // Every Object needs a Transform
        public Transform Transform { get; set; } = new Transform();

        public GameObject()
        {
            ActiveObjects.Add(this); // Add Object to list of Objects for later rendering (placeholder)
            AddComponent(Transform); // Add Transform component to the components list
        }

        /// <summary>
        /// Runs GameObject update logic.
        /// </summary>
        public static void GameObjectUpdate(float deltaTime)
        {
            // Create a copy of ActiveObjects to avoid modifying the collection during iteration
            List<GameObject> objectsToUpdate = new List<GameObject>(ActiveObjects);

            // Loop through every GameObject
            foreach (GameObject obj in objectsToUpdate)
            {
                // Create a copy of the Components list to avoid modification issues during iteration
                List<GameComponent> componentsToUpdate = new List<GameComponent>(obj.Components);

                // Loop through every GameComponent in GameObject
                foreach (GameComponent comp in componentsToUpdate)
                {
                    comp.Update(deltaTime);
                }
            }

            // Process deferred destruction
            ProcessDestructionQueue();
        }

        /// <summary>
        /// Processes the queue of GameObjects to destroy.
        /// </summary>
        private static void ProcessDestructionQueue()
        {
            while (_objectsToDestroy.Count > 0)
            {
                GameObject obj = _objectsToDestroy.Dequeue();
                ActiveObjects.Remove(obj);

                // Run destruction logic for all components
                foreach (GameComponent comp in obj.Components)
                {
                    comp.OnDestroy();
                }

                obj.Components.Clear();

                Console.WriteLine("Destroyed GameObject: " + obj);
            }
        }

        /// <summary>
        /// Runs all GameObject initialization logic.
        /// </summary>
        public void GameObjectStart()
        {
            // Create a copy of the Components list to avoid modification issues during iteration
            List<GameComponent> componentsToStart = new List<GameComponent>(Components);

            // Loop through the copy of components
            foreach (GameComponent comp in componentsToStart)
            {
                comp.Start();
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
        /// Destroys the GameObject, removing it from the ActiveObjects list and cleaning up its components.
        /// </summary>
        public void DestroyDeferred()
        {
            // Add the GameObject to the destruction queue
            _objectsToDestroy.Enqueue(this);
        }
    }
}
