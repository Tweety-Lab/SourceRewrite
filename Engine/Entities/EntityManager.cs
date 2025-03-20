using SourceRewrite.Maps;

namespace SourceRewrite.Entities
{
    public static class EntityManager
    {
        // Root of the Entity hierarchy
        public static BaseEntity Root { get; private set; }

        // Map container Entity
        public static BaseEntity MapContainer { get; private set; }

        // Global entities container
        public static BaseEntity GlobalContainer { get; private set; }

        /// <summary>
        /// List of Entities that persist across maps.
        /// </summary>
        public static List<BaseEntity> GlobalEntities = new List<BaseEntity>();

        // Queue for deferred destruction of Entities
        public static Queue<BaseEntity> EntitiesToDestroy = new Queue<BaseEntity>();

        static EntityManager()
        {
            // Create root entity
            Root = new BaseEntity { Name = "Root" };

            // Create container for global entities
            GlobalContainer = new BaseEntity { Name = "GlobalEntities", Parent = Root };

            // Create container for map entities (will be populated when a map is loaded)
            MapContainer = new BaseEntity { Name = "MapContainer", Parent = Root };
        }

        /// <summary>
        /// Add a Entity to the global container
        /// </summary>
        public static void AddGlobalEntity(BaseEntity entity)
        {
            entity.Parent = GlobalContainer;
            GlobalEntities.Add(entity);
        }

        /// <summary>
        /// Add a Entity to the map container
        /// </summary>
        public static void AddMapEntity(BaseEntity entity)
        {
            entity.Parent = MapContainer;
            if (MapSystem.CurrentMap != null)
            {
                MapSystem.CurrentMap.Entities.Add(entity);
            }
        }

        // Track if global entities have already been started
        public static bool GlobaEntitiesStarted = false;

        /// <summary>
        /// Process global Entities that persist across maps
        /// </summary>
        public static void StartGlobalEntities()
        {
            List<BaseEntity> globalEntitiesToProcess = new List<BaseEntity>(GlobalEntities);
            if (globalEntitiesToProcess.Count > 0 && !GlobaEntitiesStarted)
            {
                foreach (BaseEntity globalEntity in globalEntitiesToProcess)
                {
                    globalEntity.Start();
                }
            }
        }

        /// <summary>
        /// Runs Entity update logic for every Entity.
        /// </summary>
        public static void UpdateAllEntities()
        {
            // Update the root entity and all its children recursively
            UpdateEntityRecursive(Root);

            // Process deferred destruction
            ProcessDestructionQueue();
        }

        /// <summary>
        /// Runs Entity start logic for every Entity.
        /// </summary>
        public static void StartAllEntities()
        {
            // Start the root entity and all its children recursively
            StartEntityRecursive(Root);
        }

        /// <summary>
        /// Update a Entity and all its children recursively
        /// </summary>
        public static void UpdateEntityRecursive(BaseEntity obj)
        {
            // Update current object only if enabled
            if (obj.IsEnabled)
                obj.Update();

            // Update all children recursively
            foreach (BaseEntity child in obj.Children.ToList())
            {
                UpdateEntityRecursive(child);
            }
        }


        /// <summary>
        /// Start a Entity and all its children recursively
        /// </summary>
        public static void StartEntityRecursive(BaseEntity obj)
        {
            // Start current object only if enabled
            if (obj.IsEnabled)
                obj.Start();

            // Start all children recursively
            foreach (BaseEntity child in obj.Children.ToList())
            {
                StartEntityRecursive(child);
            }
        }

        /// <summary>
        /// Processes the queue of Entities to destroy.
        /// </summary>
        public static void ProcessDestructionQueue()
        {
            while (EntitiesToDestroy.Count > 0)
            {
                BaseEntity obj = EntitiesToDestroy.Dequeue();

                // Remove from parent
                if (obj.Parent != null)
                {
                    obj.Parent.Children.Remove(obj);
                    obj.Parent = null;
                }

                // Destroy all children recursively
                foreach (BaseEntity child in obj.Children.ToList())
                {
                    child.DestroyDeferred();
                }

                // Remove from map's Entities list if applicable
                if (MapSystem.CurrentMap != null)
                {
                    MapSystem.CurrentMap.Entities.Remove(obj);
                }

                // Remove from GlobalEntities if applicable
                GlobalEntities.Remove(obj);

                Console.WriteLine("Destroyed Entity: " + obj.Name);
            }
        }

        /// <summary>
        /// Create a new global Entity that persists across maps
        /// </summary>
        public static BaseEntity CreateGlobalEntity(string name)
        {
            BaseEntity global = new BaseEntity();
            global.Name = name;
            EntityManager.AddGlobalEntity(global);
            return global;
        }

        /// <summary>
        /// Helper function to print the root and its children in a tree structure
        /// </summary>
        public static void PrintEntityHierarchy(BaseEntity root, string indent = "")
        {
            // Print the current Entity's name
            Console.WriteLine(indent + root.Name);

            // Recursively print all child Entities
            foreach (var child in root.Children)
            {
                PrintEntityHierarchy(child, indent + "  ");  // Indent each child to represent the hierarchy
            }
        }
    }
}
