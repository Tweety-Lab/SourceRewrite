using FileFormats.BSP;
using System.Numerics;
using SourceRewrite.AssetTypes;
using SourceRewrite.Components;
using SourceRewrite.Objects;
using FileFormats.KeyValues;
using System.Reflection;
using SourceRewrite.Maths;
using SourceRewrite.Files;

namespace SourceRewrite.Maps
{
    public class Map
    {
        // List of GameObjects in the map
        public List<GameObject> GameObjects = new List<GameObject>();

        public Map(string bspPath)
        {
            // Read map data
            BSPReader reader = new BSPReader(bspPath);

            // Get map data (Lumps)
            Lump GameObjects = reader.GetLump(LumpType.LUMP_GAME_OBJECTS);

            Lump Vertices = reader.GetLump(LumpType.LUMP_VERTEXES);
            Lump Indices = reader.GetLump(LumpType.LUMP_INDICES);
            Lump Materials = reader.GetLump(LumpType.LUMP_SOLID_MATERIALS);

            // Create the map from Lump data
            CreateGeometry(Vertices, Indices, Materials);
            CreateGameObjects(GameObjects);

            SpawnPlayerController();
        }

        // Create Geometry from Lump data
        public void CreateGeometry(Lump vertices, Lump indices, Lump materials)
        {
            // Convert Lump data to arrays
            float[] verticesData = (float[]) vertices.Data;
            uint[] indicesData = (uint[]) indices.Data;
            string[] materialsData = (string[])materials.Data;

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

            // Create a GameObject to house the meshrenderer
            GameObject mapGeometry = new GameObject();

            // Add MeshRenderer to GameObject
            mapGeometry.AddComponent(mapGeometryRenderer);

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

                gameObject.Transform.Position = (Vector3) positionKeyValue.Value;
                gameObject.Transform.Rotation = MathsHelper.EulerToQuaternion((Vector3) rotationKeyValue.Value);
                gameObject.Transform.Scale = (Vector3) scaleKeyValue.Value;

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
            new Map(path);
        }
    }
}