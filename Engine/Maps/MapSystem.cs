using FileFormats.BSP;
using System.Numerics;
using SourceRewrite.AssetTypes;
using SourceRewrite.Components;
using SourceRewrite.Objects;
using FileFormats.KeyValues;
using SourceRewrite.Maths;
using SourceRewrite.Files;

namespace SourceRewrite.Maps
{
    public class Map
    {
        // List of GameObjects in the map
        public List<GameObject> GameObjects = new List<GameObject>();

        public Transform WorldTransform = new Transform(); // Prefab Transform

        private string bspPath = "";

        public Map(string inputBspPath)
        {
            bspPath = inputBspPath;
        }

        // Load The map to the world
        public void LoadMap()
        {
            // Read map data
            BSPReader reader = new BSPReader(bspPath);

            // Get map data (Lumps)
            Lump GameObjectsLump = reader.GetLump(LumpType.LUMP_GAME_OBJECTS);

            Lump VerticesLump = reader.GetLump(LumpType.LUMP_VERTEXES);
            Lump IndicesLump = reader.GetLump(LumpType.LUMP_INDICES);
            Lump MaterialsLump = reader.GetLump(LumpType.LUMP_SOLID_MATERIALS);

            // Create the map from Lump data
            CreateGeometry(VerticesLump, IndicesLump, MaterialsLump);
            CreateGameObjects(GameObjectsLump);

            SpawnPlayerController();

            // Run start logic on all gameobjects
            foreach (GameObject gameObject in GameObjects)
            {
                gameObject.GameObjectStart();
            }
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

                // Create component
                Type componentType = Type.GetType(componentNamespace);
                GameComponent gameComponent = (GameComponent) Activator.CreateInstance(componentType);

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
        public static void LoadMap(string path)
        {
            Map map = new Map(path);
            map.LoadMap();
        }
    }
}