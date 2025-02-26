using FileFormats.BSP;
using System.Numerics;
using SourceRewrite.AssetTypes;
using SourceRewrite.Components;
using SourceRewrite.Objects;
using FileFormats.KeyValues;
using System.Reflection;

// Beware those trying to read this, it might try to fight back.
// This code is so horrible, rewrite from scratch.
// REWRITE PRIORITY: HIGH!!!
namespace SourceRewrite.Maps
{
    public static class MapSystem
    {
        public static void LoadMap(string path)
        {
            if (!File.Exists(path))
                return;

            CreateBspGeometry(path); // Create Brush Geo
            CreateCamera(); // Create a test camera for viewing

            BSPReader reader = new BSPReader(path); // Begin reading BSP

            List<KeyValuesFormat> gameObjects = GetGameObjectKeyValues(reader);
            CreateGameObjects(gameObjects); // Create GameObjects

            reader.Dispose(); // Close the reader
        }

        // Create a BSPMesh and render it from BSP Vertices/Indices Lump
        private static void CreateBspGeometry(string path)
        {
            var bspMesh = new BSPMesh(path);
            var bspRenderer = new MeshRenderer();

            bspRenderer.Mesh = bspMesh; // Set the Mesh

            var bspGeometry = new GameObject();
            bspGeometry.AddComponent(bspRenderer);
        }

        // Create a Camera
        private static void CreateCamera()
        {
            var cameraObject = new GameObject();
            cameraObject.AddComponent(new Camera());
            cameraObject.AddComponent(new CameraController());
        }

        // Get the BSP's GameObjects as KeyValuesFormats
        private static List<KeyValuesFormat> GetGameObjectKeyValues(BSPReader reader)
        {
            // Read data from BSP
            string[] gameObjectData = reader.GetLumpData<string[]>(LumpType.LUMP_GAME_OBJECTS);
            List<KeyValuesFormat> output = new List<KeyValuesFormat>();

            // Loop through every Game Object
            foreach(var gameObject in gameObjectData)
            {
                // Read KeyValues
                KeyValuesFormat gameObjectKeyValues = new KeyValuesFormat(gameObject);
                output.Add(gameObjectKeyValues);
            }

            return output;
        }

        // Create GameObjects from a list of them
        private static void CreateGameObjects(List<KeyValuesFormat> gameObjects)
        {
            // Loop through every GameObject
            foreach (KeyValuesFormat gameObjectKV in gameObjects)
            {
                GameObject gameObject = new GameObject(); // Make the GameObject

                // Get the specified Transforms
                KeyValue positionKV = gameObjectKV.GetKeyValue("position");
                KeyValue scaleKV = gameObjectKV.GetKeyValue("scale");
                KeyValue rotationKV = gameObjectKV.GetKeyValue("rotation");

                // Apply Transforms
                Vector3 position = positionKV.GetValueAsType<Vector3>();
                Vector3 scale = scaleKV.GetValueAsType<Vector3>();
                Vector3 rotation = rotationKV.GetValueAsType<Vector3>();

                gameObject.Transform.Position = position;
                gameObject.Transform.Scale = scale;
                gameObject.Transform.Rotation = Maths.MathsHelper.EulerToQuaternion(rotation);

                CreateGameComponents(gameObjectKV, gameObject); // Populate with GameComponents defined in BSP
            }
        }

        // Populate a GameObject with components from KeyValues
        private static void CreateGameComponents(KeyValuesFormat gameObjectKV, GameObject targetObject)
        {
            // Get Game Components Parent Key
            ParentKey gameComponentsPK = gameObjectKV.ParentKeys[0].ChildParentKeys[0];

            if (gameComponentsPK == null) return;

            // Loop through every Game Component
            foreach (ParentKey gameComponentPK in gameComponentsPK.ChildParentKeys)
            {
                // We store namespaces as SourceRewrite_Components_CompName in BSP
                string componentNameSpace = gameComponentPK.Name.Replace('_', '.');

                // Convert from namespace to Type
                Type componentType = Type.GetType(componentNameSpace);

                Console.WriteLine(componentNameSpace);

                // Create GameComponent
                GameComponent gameComponent = (GameComponent) Activator.CreateInstance(componentType);

                // Add Component to GameObject
                targetObject.AddComponent(gameComponent);

                // Init properties
                SetGameComponentProperties(gameComponentPK, gameComponent);
            }
        }

        private static void SetGameComponentProperties(ParentKey gameComponentPK, GameComponent gameComponent)
        {
            // Loop through every Property
            foreach(KeyValue propertyKV in gameComponentPK.ChildKeyValues)
            {
                // Get Property Data
                string propertyName = propertyKV.Key;
                string propertyValue = propertyKV.GetValueAsType<string>();

                // Set the Property
                FieldInfo field = gameComponent.GetType().GetField(propertyName);
                field.SetValue(gameComponent, propertyValue);
            }
        }
    }
}