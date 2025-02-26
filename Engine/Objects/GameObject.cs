using SourceRewrite.Components;

namespace SourceRewrite.Objects
{
    public class GameObject
    {
        // Every object that exists
        public static List<GameObject> ActiveObjects { get; private set; } = new List<GameObject>();

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
        /// Runs all GameObject update logic.
        /// </summary>
        public static void GameObjectUpdate(float deltaTime)
        {
            // Loop through every GameObject
            foreach(GameObject obj in ActiveObjects)
            {
                // Loop through every GameComponent in GameObject
                foreach(GameComponent comp in obj.Components)
                {
                    comp.Update(deltaTime);
                }
            }
        }

        /// <summary>
        /// Runs all GameObject initialization logic.
        /// </summary>
        public static void GameObjectStart()
        {
            // Loop through every GameObject
            foreach (GameObject obj in ActiveObjects)
            {
                // Loop through every GameComponent in GameObject
                foreach (GameComponent comp in obj.Components)
                {
                    comp.Start();
                }
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
    }
}
