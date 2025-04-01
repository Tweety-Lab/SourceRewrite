using SourceRewrite.Attributes;
using SourceRewrite.Maps;

namespace SourceRewrite.Entities
{
    public static class EntityManager
    {
        // Root of the Entity hierarchy
        public static BaseEntity Root { get; }

        // Map container Entity
        public static BaseEntity MapContainer { get; }

        // Global entities container
        public static BaseEntity GlobalContainer { get; }

        /// <summary>
        /// List of Entities that persist across maps.
        /// </summary>
        public static List<BaseEntity> GlobalEntities = new List<BaseEntity>();

        // Queue for deferred destruction of Entities
        public static Queue<BaseEntity> EntitiesToDestroy = new Queue<BaseEntity>();

        // Tracks if golbal entities have been started or not
        private static bool _globalEntitiesStarted = false;

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
            if (!_globalEntitiesStarted)
            {
                List<BaseEntity> globalEntitiesToProcess = new List<BaseEntity>(GlobalEntities);
                foreach (BaseEntity globalEntity in globalEntitiesToProcess)
                {
                    globalEntity.Start();
                }
                _globalEntitiesStarted = true;
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
            // Start global entities only if they haven't been started yet
            StartGlobalEntities();

            // Start the map entity and all its children recursively
            StartEntityRecursive(MapContainer);
        }

        /// <summary>
        /// Start an Entity and all its children recursively
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
        /// Enable an Entity and all its children recursively
        /// </summary>
        /// <param name="obj"></param>
        public static void EnableEntityRecursive(BaseEntity obj)
        {
            obj.IsEnabled = true;
            foreach (BaseEntity child in obj.Children.ToList())
            {
                EnableEntityRecursive(child);
            }
        }

        /// <summary>
        /// Disable an Entity and all its children recursively
        /// </summary>
        /// <param name="obj"></param>
        public static void DisableEntityRecursive(BaseEntity obj)
        {
            obj.IsEnabled = false;
            foreach (BaseEntity child in obj.Children.ToList())
            {
                DisableEntityRecursive(child);
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

                DeveloperConsole.Msg("Destroyed Entity: " + obj.Name);
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

        public static void CreateEntityFromName(string name)
        {
            // Find entity type with the matching name
            object entityType = AttributeManager.GetTypeByAttributeValue<EntityAttribute, string>("ClassName", name);

            if (entityType != null)
            {
                // Create Entity
                BaseEntity entity = (BaseEntity)Activator.CreateInstance((Type)entityType);
                entity.Name = name;
                AddMapEntity(entity);

                // Start the Entity
                entity.Start();
            }
            else
            {
                DeveloperConsole.Error($"Attempted to create unknown entity type {name}!");
            }
        }
    }
}
