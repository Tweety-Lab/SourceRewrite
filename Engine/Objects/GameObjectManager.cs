using SourceRewrite.Components;
using SourceRewrite.Maps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SourceRewrite.Objects
{
    static class GameObjectManager
    {
        // Queue for deferred destruction of GameObjects
        public static Queue<GameObject> ObjectsToDestroy = new Queue<GameObject>();

        /// <summary>
        /// Runs GameObject update logic for every GameObject.
        /// </summary>
        public static void GameObjectUpdate()
        {
            // Create a copy of AllGameObjects to avoid modifying the collection during iteration
            List<GameObject> objectsToUpdate = new List<GameObject>(MapSystem.AllGameObjects);

            // Loop through every GameObject
            foreach (GameObject obj in objectsToUpdate)
            {
                // Create a copy of the Components list to avoid modification issues during iteration
                List<GameComponent> componentsToUpdate = new List<GameComponent>(obj.Components);

                // Loop through every GameComponent in GameObject
                foreach (GameComponent comp in componentsToUpdate)
                {
                    comp.Update();
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
            while (ObjectsToDestroy.Count > 0)
            {
                GameObject obj = ObjectsToDestroy.Dequeue();
                MapSystem.AllGameObjects.Remove(obj);
                MapSystem.CurrentMap.GameObjects.Remove(obj);

                // Run destruction logic for all components
                foreach (GameComponent comp in obj.Components)
                {
                    comp.OnDestroy();
                }

                obj.Components.Clear();

                Console.WriteLine("Destroyed GameObject: " + obj);
            }
        }
    }
}
