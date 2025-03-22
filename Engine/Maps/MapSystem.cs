using FileFormats.BSP;
using System.Numerics;
using SourceRewrite.AssetTypes;
using SourceRewrite.Entities;
using FileFormats.KeyValues;
using SourceRewrite.Maths;
using SourceRewrite.Files;
using SourceRewrite.Modding;
using System.ComponentModel;

namespace SourceRewrite.Maps
{
    public class Map
    {
        // List of Entities in the map
        public List<BaseEntity> Entities = new List<BaseEntity>();

        // Map Entity that contains all map entities
        public BaseEntity MapRootEntity { get; }

        // Determines if Entities should start enabled
        public bool EntitiesEnabled { get; set; }

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

            // Get map data (Lumps)
            Lump EntitiesLump = reader.GetLump(LumpType.LUMP_ENTITIES);

            Lump VerticesLump = reader.GetLump(LumpType.LUMP_VERTEXES);
            Lump IndicesLump = reader.GetLump(LumpType.LUMP_INDICES);
            Lump MaterialsLump = reader.GetLump(LumpType.LUMP_SOLID_MATERIALS);

            // Create the map from Lump data
            CreateGeometry(VerticesLump, IndicesLump, MaterialsLump);
            CreateEntities(EntitiesLump);

            // Disable all Entities if EntitiesEnabled is false
            if (EntitiesEnabled == false)
            {
                EntityManager.DisableEntityRecursive(MapRootEntity);
            }
            
            // Re-Enable Entities with AlwaysExecute attribute
            foreach (BaseEntity entity in Entities)
            {
                if (Attributes.AttributeCache.AlwaysExecuteEntitiesTypes.Contains(entity.GetType()))
                {
                    entity.IsEnabled = true;
                }
            }

            // Start all Entities (Global and Map)
            EntityManager.StartAllEntities();

            // Free the BSP
            reader.Dispose();
        }

        // Create Geometry from Lump data
        public void CreateGeometry(Lump vertices, Lump indices, Lump materials)
        {
            // Convert Lump data to arrays
            float[] verticesData = (float[]) vertices.Data;
            uint[] indicesData = (uint[]) indices.Data;
            string[] materialsData = (string[])materials.Data;

            // Create a Entity to house the MeshEntity
            MeshEntity mapGeometry = new MeshEntity();

            // Assign Entity Name
            mapGeometry.Name = "MapGeometry";

            // For now, we just use the first material defined in the lump
            Material placeHolderMaterial = FileSystem.GetMaterial(materialsData[0]);

            // Create a MeshAsset
            Mesh mapGeometryMesh = new Mesh();

            // Populate the MeshAsset with lump data
            mapGeometryMesh.Vertices = verticesData;
            mapGeometryMesh.Indices = indicesData;
            mapGeometryMesh.Material = placeHolderMaterial;

            mapGeometry.Mesh = mapGeometryMesh;

            // Add the Entity to the map's Entities list
            Entities.Add(mapGeometry);
            mapGeometry.Parent = MapRootEntity; // Ensure parent is set correctly
        }

        // Create Entities from Lump data
        public void CreateEntities(Lump entities)
        {
            // Convert Lump data to array
            string[] entitiesData = (string[]) entities.Data;

            if (entitiesData == null)
                return;

            // We store Entities in a KeyValues format
            foreach (string entityString in entitiesData)
            {
                // Create Entity from KeyValues
                KeyValuesFormat entityKeyValues = new KeyValuesFormat(entityString);

                // Get Entity data as KeyValues
                KeyValue positionKeyValue = entityKeyValues.GetKeyValue("position");
                KeyValue rotationKeyValue = entityKeyValues.GetKeyValue("rotation");
                KeyValue scaleKeyValue = entityKeyValues.GetKeyValue("scale");

                // Create Entity with correct class
                string entityNamespace = (string)entityKeyValues.GetKeyValue("classname").Value;
                entityNamespace = entityNamespace.Replace("_", ".");

                // Attempt to find type from across all loaded assemblies
                Type entityType = ModSystem.FindTypeInLoadedAssemblies(entityNamespace);

                if (entityType == null)
                {
                    Console.WriteLine($"Could not find type: '{entityNamespace}' ");
                    entityType = typeof(BaseEntity); // Fallback to BaseEntity if not found
                }

                Console.WriteLine($"Creating entity of type: {entityNamespace}");

                // Dynamically create the correct entity type
                BaseEntity entity = (BaseEntity)Activator.CreateInstance(entityType);

                // Process Entity properties
                foreach (KeyValue propertyKeyValue in entityKeyValues.ParentKeys[0].ChildKeyValues)
                {
                    entity.SetProperty(propertyKeyValue.Key, propertyKeyValue.Value);
                }

                // Assign Entity Name
                entity.Name = entity.GetType().ToString();

                // Adjust position relative to the WorldTransform's Position
                Vector3 adjustedPosition = (Vector3)positionKeyValue.Value;
                entity.Transform.Position = adjustedPosition;

                // Adjust rotation relative to WorldTransform's Rotation
                Quaternion adjustedRotation = MathsHelper.EulerToQuaternion((Vector3)rotationKeyValue.Value);
                entity.Transform.Rotation = adjustedRotation;

                Entities.Add(entity);
                entity.Parent = MapRootEntity; // Ensure parent is set correctly

                EntityManager.PrintEntityHierarchy(EntityManager.Root);
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