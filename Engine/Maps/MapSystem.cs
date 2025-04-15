using FileFormats.BSP;
using System.Numerics;
using SourceRewrite.AssetTypes;
using SourceRewrite.Entities;
using FileFormats.KeyValues;
using SourceRewrite.Files;
using SourceRewrite.Attributes;
using FileFormats.BSP.IO;
using SourceRewrite.PhysicsSystem;
using Silk.NET.Input;

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

                // --- UV Generation ---
                sideMesh.UVs = new float[side.Vertices.Length * 2]; // 2 floats per vertex: U, V

                // Get the base texture of side
                Texture basetexture = sideMesh.Material.Shader.GetParameter<Texture>("basetexture");

                if (basetexture != null)
                {
                    for (int i = 0; i < side.Vertices.Length / 3; i++)
                    {
                        // Get the 3D position from the float[] vertices
                        float x = side.Vertices[i * 3 + 0];
                        float y = side.Vertices[i * 3 + 1];
                        float z = side.Vertices[i * 3 + 2];

                        Vector2 uv = ComputeUV(
                            new Vector3(x, y, z),
                            side.UAxis,
                            side.VAxis,
                            basetexture.Width,
                            basetexture.Height
                        );

                        // Store into the flat UV array
                        sideMesh.UVs[i * 2 + 0] = uv.X;  // U
                        sideMesh.UVs[i * 2 + 1] = uv.Y;  // V
                    }
                }


                sideGeometry.Mesh = sideMesh;

                // Register Physics for Geometry
                sideGeometry.PhysicsBody.IsStatic = true; // Set to static for map geometry
                sideGeometry.PhysicsBody.CollisionMesh = sideMesh;

                sideGeometry.PhysicsInitNormal();

                // Add the Entity to the map's Entities list
                Entities.Add(sideGeometry);
                sideGeometry.Parent = MapRootEntity; // Ensure parent is set correctly
            }
        }
        Vector2 ComputeUV(Vector3 worldPos, BSPUVAxis uaxis, BSPUVAxis vaxis, float textureWidth, float textureHeight)
        {
            float uOffset = uaxis.Axis.W / 4;
            float vOffset = vaxis.Axis.W / 4;

            float u = (Vector3.Dot(worldPos, new Vector3(uaxis.Axis.X, uaxis.Axis.Y, uaxis.Axis.Z)) + uOffset) / uaxis.Scale;
            float v = (Vector3.Dot(worldPos, new Vector3(vaxis.Axis.X, vaxis.Axis.Y, vaxis.Axis.Z)) + vOffset) / vaxis.Scale;

            // Now normalize texture size
            u *= 1.0f / textureWidth;
            v *= 1.0f / textureHeight;

            return new Vector2(u, v);
        }

        // Create Entities from Lump data
        public void CreateEntities(BSPEntity[] entitiesData)
        {
            if (entitiesData == null)
                return;

            foreach (BSPEntity bspEntity in entitiesData)
            {
                // Parse entity properties
                KeyValuesFormat entityKeyValues = new KeyValuesFormat(bspEntity.KeyValuesString);
                string className = (string)entityKeyValues.GetKeyValue("classname").Value;

                // Skip worldspawn as it's handled separately
                if (className == "worldspawn")
                    continue;

                // Determine if this is a brush entity
                bool isBrushEntity = bspEntity.BrushSides != null && bspEntity.BrushSides.Length > 0;

                if (isBrushEntity)
                {
                    CreateBrushEntity(className, entityKeyValues, bspEntity.BrushSides);
                }
                else
                {
                    CreatePointEntity(className, entityKeyValues);
                }
            }
        }
        private void CreateBrushEntity(string className, KeyValuesFormat entityKeyValues, BSPPlane[] brushSides)
        {
            // Find the Type that matches the ClassName
            Type entityType = AttributeManager.GetTypeByAttributeValue<EntityAttribute, string>("ClassName", className);

            if (entityType == null)
            {
                Console.WriteLine($"Could not find type: '{className}'");
                return;
            }

            BrushEntity brushEntity = (BrushEntity)Activator.CreateInstance(entityType);
            brushEntity.Name = entityKeyValues.ParentKeys[0].Name;

            List<Mesh> brushMeshes = new List<Mesh>();
            foreach (BSPPlane side in brushSides)
            {
                // Skip NODRAW surfaces
                if (side.MaterialName.Equals("tools/toolsnodraw", StringComparison.OrdinalIgnoreCase))
                    continue;

                // Create a MeshAsset
                Mesh sideMesh = new Mesh
                {
                    Vertices = side.Vertices,
                    Indices = side.Indices,
                    Material = FileSystem.GetMaterial(side.MaterialName)
                };

                // Generate UVs using the same logic as world geometry
                sideMesh.UVs = new float[side.Vertices.Length * 2]; // 2 floats per vertex: U, V
                Texture baseTexture = sideMesh.Material?.Shader?.GetParameter<Texture>("basetexture");

                if (baseTexture != null)
                {
                    for (int i = 0; i < side.Vertices.Length / 3; i++)
                    {
                        // Get the 3D position from the float[] vertices
                        float x = side.Vertices[i * 3 + 0];
                        float y = side.Vertices[i * 3 + 1];
                        float z = side.Vertices[i * 3 + 2];

                        Vector2 uv = ComputeUV(
                            new Vector3(x, y, z),
                            side.UAxis,
                            side.VAxis,
                            baseTexture.Width,
                            baseTexture.Height
                        );

                        // Store into the flat UV array
                        sideMesh.UVs[i * 2 + 0] = uv.X;  // U
                        sideMesh.UVs[i * 2 + 1] = uv.Y;  // V
                    }
                }

                brushMeshes.Add(sideMesh);
            }

            brushEntity.Brush = brushMeshes;

            // Set properties (excluding transform-related ones)
            foreach (KeyValue propertyKeyValue in entityKeyValues.ParentKeys[0].ChildKeyValues)
            {
                string key = propertyKeyValue.Key.ToLower();

                // Skip transform properties
                if (key == "origin" || key == "angles" || key == "scale")
                    continue;

                // Handle connections
                if (key.StartsWith("connection_"))
                {
                    EntityIOConnection io = EntityIOUtility.ParseIOString($"{propertyKeyValue.Key} \"{propertyKeyValue.Value}\"");
                    brushEntity.Outputs.Add(io);
                    continue;
                }

                brushEntity.SetProperty(propertyKeyValue.Key, propertyKeyValue.Value);
            }

            // Add to entity list
            Entities.Add(brushEntity);
            brushEntity.Parent = MapRootEntity;

            Console.WriteLine($"Creating brush entity of type: {className}");
        }

        private void CreatePointEntity(string className, KeyValuesFormat entityKeyValues)
        {
            // Find the Type that matches the ClassName
            Type entityType = AttributeManager.GetTypeByAttributeValue<EntityAttribute, string>("ClassName", className);

            if (entityType == null)
            {
                Console.WriteLine($"Could not find type: '{className}'");
                return;
            }

            Console.WriteLine($"Creating point entity of type: {className}");

            // Create point entity
            PointEntity pointEntity = (PointEntity)Activator.CreateInstance(entityType);
            pointEntity.Name = entityKeyValues.ParentKeys[0].Name;

            // Process properties
            foreach (KeyValue propertyKeyValue in entityKeyValues.ParentKeys[0].ChildKeyValues)
            {
                string key = propertyKeyValue.Key.ToLower();

                // Handle transform properties
                if (key == "position")
                {
                    pointEntity.Transform.Position = (Vector3)propertyKeyValue.Value;
                    continue;
                }
                else if (key == "rotation")
                {
                    pointEntity.Transform.Rotation = Math.EulerToQuaternion((Vector3)propertyKeyValue.Value);
                    continue;
                }
                else if (key == "scale")
                {
                    pointEntity.Transform.Scale = (Vector3)propertyKeyValue.Value;
                    continue;
                }
                else if (key.StartsWith("connection_"))
                {
                    EntityIOConnection io = EntityIOUtility.ParseIOString($"{propertyKeyValue.Key} \"{propertyKeyValue.Value}\"");
                    pointEntity.Outputs.Add(io);
                    continue;
                }

                // Set regular properties
                pointEntity.SetProperty(propertyKeyValue.Key, propertyKeyValue.Value);
            }

            // Add to entity list
            Entities.Add(pointEntity);
            pointEntity.Parent = MapRootEntity;
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