using FileFormats.BSP;
using System.Numerics;
using SourceRewrite.AssetTypes;
using SourceRewrite.Entities;
using FileFormats.KeyValues;
using SourceRewrite.Maths;
using SourceRewrite.Files;
using SourceRewrite.Attributes;
using FileFormats.BSP.IO;
using SourceRewrite.PhysicsSystem;

namespace SourceRewrite.Maps
{
    public class Map
    {
        // List of Entities in the map
        public List<BaseEntity> Entities = new List<BaseEntity>();

        // Map Entity that contains all map entities
        public BaseEntity MapRootEntity { get; }

        // Determines if Entities should start enabled
        public bool EntitiesEnabled { get; set; } = true;

        public string BSPFilePath;

        public Map(string inputBspPath)
        {
            BSPFilePath = inputBspPath;

            // Create map root entity
            MapRootEntity = new BaseEntity();
            MapRootEntity.Name = "Map_" + System.IO.Path.GetFileNameWithoutExtension(inputBspPath);
            MapRootEntity.Parent = EntityManager.MapContainer;
        }

        // Unload the map from the world
        public void UnloadMap()
        {
            // Destroy Entites in Map Container
            foreach (BaseEntity entity in MapRootEntity.Parent.Children)
            {
                entity.DestroyDeferred();
            }

            Entities.Clear();

            // Process destruction queue immediately to ensure cleanup
            EntityManager.ProcessDestructionQueue();
        }

        // Load The map to the world
        public void LoadMap()
        {
            // Read map data
            BSPReader reader = new BSPReader(BSPFilePath);

            // Create the map from Lump data
            CreateGeometry(reader.GetLumpData<BSPPlane>(BSPLumpType.LUMP_PLANES));
            CreateEntities(reader.GetLumpData<BSPEntity>(BSPLumpType.LUMP_ENTITIES));

            // Disable all Entities if EntitiesEnabled is false
            if (EntitiesEnabled == false)
            {
                EntityManager.DisableEntityRecursive(MapRootEntity);
            }

            // Get all types with AlwaysExecuteAttribute
            var alwaysExecuteTypes = new HashSet<Type>(AttributeManager.GetTypesWithAttribute<AlwaysExecuteAttribute>());

            // Enable entities that match the types with the AlwaysExecuteAttribute
            foreach (BaseEntity entity in Entities)
            {
                if (alwaysExecuteTypes.Contains(entity.GetType()))
                {
                    entity.IsEnabled = true;
                }
            }

            // Start all Entities (Global and Map)
            EntityManager.StartAllEntities();
        }

        // Create Geometry from Lump data
        public void CreateGeometry(BSPPlane[] sides)
        {
            foreach (BSPPlane side in sides)
            {
                // Create a Entity to house the MeshEntity
                MeshEntity sideGeometry = new MeshEntity();

                // Assign Entity Name
                sideGeometry.Name = "MapGeometry";

                // Create a MeshAsset
                Mesh sideMesh = new Mesh();

                // Populate the MeshAsset with lump data
                sideMesh.Vertices = side.Vertices;
                sideMesh.Indices = side.Indices;
                sideMesh.Material = FileSystem.GetMaterial(side.MaterialName);

                sideGeometry.Mesh = sideMesh;

                // Register Physics for Geometry
                PhysicsBody body = new PhysicsBody();
                body.IsStatic = true; // Set to static for map geometry
                body.CollisionMesh = sideMesh;

                sideGeometry.PhysicsInitNormal(body);

                // Add the Entity to the map's Entities list
                Entities.Add(sideGeometry);
                sideGeometry.Parent = MapRootEntity; // Ensure parent is set correctly
            }
        }

        // Create Entities from Lump data
        public void CreateEntities(BSPEntity[] entitiesData)
        {
            if (entitiesData == null)
                return;

            foreach (BSPEntity bspEntity in entitiesData)
            {
                // Create Entity from KeyValues
                KeyValuesFormat entityKeyValues = new KeyValuesFormat(bspEntity.KeyValuesString);

                // Create Entity with correct class
                string entityNamespace = (string)entityKeyValues.GetKeyValue("classname").Value;

                // Find the Type that matches the ClassName
                Type entityType = AttributeManager.GetTypeByAttributeValue<EntityAttribute, string>("ClassName", entityNamespace);

                if (entityType == null)
                {
                    Console.WriteLine($"Could not find type: '{entityNamespace}' ");
                    continue;
                }

                Console.WriteLine($"Creating entity of type: {entityNamespace}");

                // Dynamically create the correct entity type
                PointEntity entity = (PointEntity)Activator.CreateInstance(entityType);

                // Process Entity properties
                foreach (KeyValue propertyKeyValue in entityKeyValues.ParentKeys[0].ChildKeyValues)
                {
                    // Skip Entity IO events
                    if (propertyKeyValue.Key.StartsWith("connection_"))
                        continue;

                    entity.SetProperty(propertyKeyValue.Key, propertyKeyValue.Value);
                }

                foreach (KeyValue propertyKeyValue in entityKeyValues.ParentKeys[0].ChildKeyValues)
                {
                    // Process Entity IO events
                    if (propertyKeyValue.Key.StartsWith("connection_"))
                    {
                        EntityIOConnection io = EntityIOUtility.ParseIOString($"{propertyKeyValue.Key} \"{propertyKeyValue.Value}\"");
                        entity.Outputs.Add(io);
                    }
                }

                // Assign Entity Name
                entity.Name = entityKeyValues.ParentKeys[0].Name;

                // Get Entity data as KeyValues
                KeyValue positionKeyValue = entityKeyValues.GetKeyValue("position");
                KeyValue rotationKeyValue = entityKeyValues.GetKeyValue("rotation");
                KeyValue scaleKeyValue = entityKeyValues.GetKeyValue("scale");

                // Adjust position relative to the WorldTransform's Position
                Vector3 adjustedPosition = (Vector3)positionKeyValue.Value;
                entity.Transform.Position = adjustedPosition;

                // Adjust rotation relative to WorldTransform's Rotation
                Quaternion adjustedRotation = MathsHelper.EulerToQuaternion((Vector3)rotationKeyValue.Value);
                entity.Transform.Rotation = adjustedRotation;

                Entities.Add(entity);
                entity.Parent = MapRootEntity; // Ensure parent is set correctly

            }
        }

        // Add a Entity to the map
        public void AddEntity(BaseEntity entity)
        {
            MapRootEntity.Children.Add(entity);
        }
    }

    public static class MapSystem
    {
        /// <summary>
        /// Currently Loaded Map.
        /// </summary>
        public static Map CurrentMap { get; private set; }

        static MapSystem()
        {
            // Ensure EntityManager is initialized
            var root = EntityManager.Root;
        }

        // Event triggered when a map is about to load
        public static event Action<Map> OnMapPreload;

        // Event triggered when a map is loaded
        public static event Action<Map> OnMapLoaded;

        // Event triggered when a map is unloaded
        public static event Action OnMapUnloaded;


        public static void LoadMap(string path)
        {
            // Unload current map if one exists
            if (CurrentMap != null)
            {
                UnloadMap();
            }

            // Create and load new map
            CurrentMap = new Map(path);
            OnMapPreload?.Invoke(CurrentMap); // Trigger Preload Event
            CurrentMap.LoadMap();


            // Trigger event after the map is loaded
            OnMapLoaded?.Invoke(CurrentMap);
        }

        public static void UnloadMap()
        {
            // Only unload if there is an open map
            if (CurrentMap == null)
                return;

            // Trigger the event before the map is unloaded
            OnMapUnloaded?.Invoke();

            // Unload Map
            CurrentMap.UnloadMap();
            CurrentMap = null;
        }
    }
}