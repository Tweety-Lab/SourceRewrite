using SourceRewrite.Components;
using SourceRewrite.Maps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SourceRewrite.Objects
{
    public static class GameObjectManager
    {
        // Root of the GameObject hierarchy
        public static GameObject Root { get; private set; }

        // Map container GameObject
        public static GameObject MapContainer { get; private set; }

        // Global objects container
        public static GameObject GlobalContainer { get; private set; }

        // Queue for deferred destruction of GameObjects
        public static Queue<GameObject> ObjectsToDestroy = new Queue<GameObject>();

        static GameObjectManager()
        {
            // Create root object
            Root = new GameObject { Name = "Root" };

            // Create container for global objects
            GlobalContainer = new GameObject { Name = "GlobalObjects", Parent = Root };

            // Create container for map objects (will be populated when a map is loaded)
            MapContainer = new GameObject { Name = "MapContainer", Parent = Root };
        }

        /// <summary>
        /// Add a GameObject to the global container
        /// </summary>
        public static void AddGlobalObject(GameObject gameObject)
        {
            gameObject.Parent = GlobalContainer;
            MapSystem.GlobalGameObjects.Add(gameObject);
        }

        /// <summary>
        /// Add a GameObject to the map container
        /// </summary>
        public static void AddMapObject(GameObject gameObject)
        {
            gameObject.Parent = MapContainer;
            if (MapSystem.CurrentMap != null)
            {
                MapSystem.CurrentMap.GameObjects.Add(gameObject);
            }
        }

        /// <summary>
        /// Runs GameObject update logic for every GameObject.
        /// </summary>
        public static void GameObjectUpdate()
        {
            // Update the root object and all its children recursively
            UpdateGameObjectRecursive(Root);

            // Process deferred destruction
            ProcessDestructionQueue();
        }

        /// <summary>
        /// Update a GameObject and all its children recursively
        /// </summary>
        private static void UpdateGameObjectRecursive(GameObject obj)
        {
            // Create a copy of the Components list to avoid modification issues during iteration
            List<GameComponent> componentsToUpdate = new List<GameComponent>(obj.Components);

            // Loop through every GameComponent in GameObject
            foreach (GameComponent comp in componentsToUpdate)
            {
                comp.Update();
            }

            // Update all children recursively
            foreach (GameObject child in obj.Children.ToList())
            {
                UpdateGameObjectRecursive(child);
            }
        }

        /// <summary>
        /// Processes the queue of GameObjects to destroy.
        /// </summary>
        private static void ProcessDestructionQueue()
        {
            while (ObjectsToDestroy.Count > 0)
            {
                GameObject obj = ObjectsToDestroy.Dequeue();

                // Remove from parent
                if (obj.Parent != null)
                {
                    obj.Parent.Children.Remove(obj);
                    obj.Parent = null;
                }

                // Destroy all children recursively
                foreach (GameObject child in obj.Children.ToList())
                {
                    child.DestroyDeferred();
                }

                // Remove from map's GameObjects list if applicable
                if (MapSystem.CurrentMap != null)
                {
                    MapSystem.CurrentMap.GameObjects.Remove(obj);
                }

                // Remove from GlobalGameObjects if applicable
                MapSystem.GlobalGameObjects.Remove(obj);

                // Run destruction logic for all components
                foreach (GameComponent comp in obj.Components)
                {
                    comp.OnDestroy();
                }

                obj.Components.Clear();
                Console.WriteLine("Destroyed GameObject: " + obj.Name);
            }
        }

        /// <summary>
        /// Create a new global GameObject that persists across maps
        /// </summary>
        public static GameObject CreateGlobalObject(string name)
        {
            GameObject global = new GameObject();
            global.Name = name;
            GameObjectManager.AddGlobalObject(global);
            return global;
        }

        /// <summary>
        /// Helper function to print the root and its children in a tree structure
        /// </summary>
        public static void PrintGameObjectHierarchy(GameObject root, string indent = "")
        {
            // Print the current GameObject's name
            Console.WriteLine(indent + root.Name);

            // Recursively print all child GameObjects
            foreach (var child in root.Children)
            {
                PrintGameObjectHierarchy(child, indent + "  ");  // Indent each child to represent the hierarchy
            }
        }
    }
}
