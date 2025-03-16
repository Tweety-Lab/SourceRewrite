using FileFormats.BSP;
using System.Numerics;
using SourceRewrite.AssetTypes;
using SourceRewrite.Components;
using SourceRewrite.Objects;
using FileFormats.KeyValues;
using SourceRewrite.Maths;
using SourceRewrite.Files;
using SourceRewrite.Modding;

namespace SourceRewrite.Maps
{
    public class Map
    {
        // List of Components that exist across every map
        public static List<GameComponent> GlobalComponents = new List<GameComponent>();

        // List of GameObjects in the map
        public List<GameObject> GameObjects = new List<GameObject>();

        // List to track GameObjects containing global components
        private List<GameObject> GlobalGameObjects = new List<GameObject>();

        public Transform WorldTransform = new Transform(); // Prefab Transform

        public string BSPFilePath = "";

        public Map(string inputBspPath)
        {
            BSPFilePath = inputBspPath;
        }

        // Unload the map from the world
        public void UnloadMap()
        {
            // First, clear any game objects and their components
            foreach (GameObject gameObject in GameObjects)
            {
                if (!GlobalGameObjects.Contains(gameObject))
                {
                    gameObject.DestroyDeferred();
                }
            }

            GameObjects.Clear();

            // Re-add global GameObjects back to the list
            foreach (GameObject globalObject in GlobalGameObjects)
            {
                GameObjects.Add(globalObject);
            }

            SpawnPlayerController();
        }

        // Load The map to the world
        public void LoadMap()
        {
            // Read map data
            BSPReader reader = new BSPReader(BSPFilePath);

            // Get map data (Lumps)
            Lump GameObjectsLump = reader.GetLump(LumpType.LUMP_GAME_OBJECTS);

            Lump VerticesLump = reader.GetLump(LumpType.LUMP_VERTEXES);
            Lump IndicesLump = reader.GetLump(LumpType.LUMP_INDICES);
            Lump MaterialsLump = reader.GetLump(LumpType.LUMP_SOLID_MATERIALS);

            // Create the map from Lump data
            CreateGeometry(VerticesLump, IndicesLump, MaterialsLump);
            CreateGameObjects(GameObjectsLump);

            SpawnPlayerController();

            // Create Global Components if they don't already exist
            CreateGlobalComponents();

            // Run start logic on all gameobjects
            foreach (GameObject gameObject in GameObjects)
            {
                gameObject.GameObjectStart();
            }

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

            // Create a GameObject to house the meshrenderer
            GameObject mapGeometry = new GameObject();

            // For now, we just use the first material defined in the lump
            Material placeHolderMaterial = FileSystem.GetMaterial(materialsData[0]);

            // Create a MeshAsset
            MeshAsset mapGeometryMesh = new MeshAsset();

            // Populate the MeshAsset with lump data
            mapGeometryMesh.Vertices = verticesData;
            mapGeometryMesh.Indices = indicesData;
            mapGeometryMesh.Material = placeHolderMaterial;

            // Create a MeshRenderer
            MeshRenderer mapGeometryRenderer = new MeshRenderer();
            mapGeometryRenderer.Mesh = mapGeometryMesh;

            // Add MeshRenderer to GameObject
            mapGeometry.AddComponent(mapGeometryRenderer);

            // Adjust position relative to the WorldTransform's Position
            mapGeometry.Transform.Position = WorldTransform.Position;

            // Adjust rotation relative to WorldTransform's Rotation
            mapGeometry.Transform.Rotation = WorldTransform.Rotation;

            // Adjust scale relative to WorldTransform's Scale
            mapGeometry.Transform.Scale = WorldTransform.Scale;

            // Add the GameObject to the map's GameObjects list
            GameObjects.Add(mapGeometry);
        }

        // Create GameObjects from Lump data
        public void CreateGameObjects(Lump gameObjects)
        {
            // Convert Lump data to array
            string[] gameObjectData = (string[]) gameObjects.Data;

            if (gameObjectData == null)
                return;

            // We store GameObjects in a KeyValues format
            foreach (string gameObjectString in gameObjectData)
            {
                // Create GameObject from KeyValues
                KeyValuesFormat gameObjectKeyValues = new KeyValuesFormat(gameObjectString);

                // Get GameObject's GameComponents from KeyValues
                List<GameComponent> gameObjectComponents = GetGameComponents(gameObjectKeyValues);

                // Get GameObject data as KeyValues
                KeyValue positionKeyValue = gameObjectKeyValues.GetKeyValue("position");
                KeyValue rotationKeyValue = gameObjectKeyValues.GetKeyValue("rotation");
                KeyValue scaleKeyValue = gameObjectKeyValues.GetKeyValue("scale");

                // Create GameObject
                GameObject gameObject = new GameObject();

                // Adjust position relative to the WorldTransform's Position
                Vector3 adjustedPosition = (Vector3)positionKeyValue.Value - WorldTransform.Position;
                gameObject.Transform.Position = adjustedPosition;

                // Adjust rotation relative to WorldTransform's Rotation
                Quaternion adjustedRotation = MathsHelper.EulerToQuaternion((Vector3)rotationKeyValue.Value) * Quaternion.Inverse(WorldTransform.Rotation);
                gameObject.Transform.Rotation = adjustedRotation;

                // Populate GameObject with gameObjectComponents
                foreach (GameComponent component in gameObjectComponents)
                {
                    gameObject.AddComponent(component);
                }

                // Add GameObject to maps GameObject list
                GameObjects.Add(gameObject);
            }
        }

        // Return List of GameComponents from GameObject KeyValues
        public List<GameComponent> GetGameComponents(KeyValuesFormat gameObjectKeyValues)
        {
            List<GameComponent> gameComponents = new List<GameComponent>();

            // Get Components from KeyValues
            List<ParentKey> componentParentKeys = gameObjectKeyValues.ParentKeys[0].GetChildParentKey("GameComponents").ChildParentKeys;

            // Process each component
            foreach (ParentKey component in componentParentKeys)
            {
                // Convert stored component name to namespace
                string componentNamespace = component.Name.Replace('_', '.');


                // Find component type using enhanced resolution
                Type componentType = ModSystem.FindTypeInLoadedAssemblies(componentNamespace);

                if (componentType == null)
                {
                    Console.WriteLine($"Warning: Could not find component type: {componentNamespace}");
                    continue;
                }

                // Validate the component type inherits from GameComponent
                if (!typeof(GameComponent).IsAssignableFrom(componentType))
                {
                    Console.WriteLine($"Warning: Type {componentNamespace} is not a GameComponent");
                    continue;
                }

                // Create component
                GameComponent gameComponent = (GameComponent)Activator.CreateInstance(componentType);

                // Process component properties
                foreach (KeyValue componentKeyValue in component.ChildKeyValues)
                {
                    // Set component properties
                    gameComponent.SetProperty(componentKeyValue.Key, componentKeyValue.Value);
                }

                // Add component to list
                gameComponents.Add(gameComponent);
            }

            return gameComponents;
        }

        public void CreateGlobalComponents(bool shouldStart = false)
        {
            // Only create global components if they don't already exist
            if (GlobalGameObjects.Count == 0)
            {
                foreach (GameComponent globalComponent in GlobalComponents)
                {
                    GameObject holder = new GameObject();
                    holder.AddComponent(globalComponent);

                    // Add to global tracking list
                    GlobalGameObjects.Add(holder);

                    // Add GameObject to maps GameObject list
                    GameObjects.Add(holder);

                    if (shouldStart)
                    {
                        holder.GameObjectStart();
                    }
                }
            }
        }


        // Placeholder for spawning a player controller in the map on load
        public void SpawnPlayerController()
        {
            // Create GameObject for player
            GameObject player = new GameObject();

            // Create it's Camera Controller
            Camera playerCamera = new Camera();
            CameraController cameraController = new CameraController();

            // Attach to Player
            player.AddComponent(playerCamera);
            player.AddComponent(cameraController);

            // Add player GameObject to the GameObjects list
            GameObjects.Add(player);
        }
    }

    public static class MapSystem
    {
        /// <summary>
        /// Currently Loaded Map.
        /// </summary>
        public static Map CurrentMap { get; private set; }

        public static void LoadMap(string path)
        {
            CurrentMap = new Map(path);
            CurrentMap.LoadMap();
        }

        public static void UnloadMap()
        {
            // Only unload if there is an open map
            if (CurrentMap == null) 
                return;

            // Unload Map
            CurrentMap.UnloadMap();
            CurrentMap = null;
        }
    }
}