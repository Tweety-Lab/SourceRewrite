using FileFormats.BSP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using SourceRewrite.AssetTypes;
using SourceRewrite.Components;
using SourceRewrite.Files;
using SourceRewrite.Objects;
using FileFormats.KeyValues;
using System.ComponentModel;
using Silk.NET.Vulkan;

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

            ProcessBspGlobalComponents(path); // Create Global Components
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

        // Process BSP Global Components
        private static void ProcessBspGlobalComponents(string path)
        {
            var reader = new BSPReader(path);
            string globalComponents = reader.GetLumpData<string>(LumpType.LUMP_GLOBAL_COMPONENTS);

            // Read the component as a KeyValue format
            KeyValuesFormat kvComponent = new KeyValuesFormat(globalComponents);

            // Get the NameSpace of the component
            string componentNamespace = kvComponent.ParentKeys[0].Name;
            componentNamespace = componentNamespace.Replace('_', '.'); // In a .bsp, namespaces are stored as source_components_component.

            // Get the Positions xyz as Strings
            string[] positionVectors = kvComponent.GetKeyValue("position").Value.ToString().Split(',');

            float vectorX = float.Parse(positionVectors[0]); // Get the X
            float vectorY = float.Parse(positionVectors[1]); // Get the Y
            float vectorZ = float.Parse(positionVectors[2]); // Get the Z

            Vector3 position = new Vector3(vectorX, vectorY, vectorZ); // Create a Position

            Console.WriteLine(componentNamespace);
            Console.WriteLine(position);

            // Create the Component
            GameComponent component = (GameComponent)ComponentCreator.CreateClassFromNamespace(componentNamespace);

            // Loop through every KeyValue
            foreach (KeyValue property in kvComponent.ParentKeys[0].ChildKeys)
            {
                string key = property.Key; // Property Name
                string value = property.Value.ToString(); // Property Value

                // If Key starts with an uppercase letter, assume it's a property
                if (char.IsUpper(key[0]))
                {
                    string[] splitValue = value.Split(':');

                    if (splitValue.Length == 2)
                    {
                        string propertyType = splitValue[0]; // Type Prefix (e.g., "M" for Mesh)
                        string propertyValue = splitValue[1]; // Actual value

                        object propertyObject = ComponentCreator.CreateProperty(propertyType, propertyValue);

                        if (propertyObject != null)
                        {
                            ComponentCreator.SetProperty(component, key, propertyObject);
                        }
                    }
                }
            }


            GameObject gameObject = new GameObject(); // Create a GameObject to hold Component
            gameObject.AddComponent(component); // Add Component

            gameObject.Transform.Position = position;

            reader.Dispose(); // Close BSPReader
        }
    }

    public static class ComponentCreator
    {
        public static object CreateClassFromNamespace(string fullClassName)
        {
            Type classType = Type.GetType(fullClassName);

            if (classType == null)
            {
                throw new InvalidOperationException($"Class with name {fullClassName} not found.");
            }

            return Activator.CreateInstance(classType);
        }

        public static object CreateProperty(string propertyType, string propertyValue)
        {
            switch (propertyType)
            {
                // Model Path
                case "M":
                    Material testMaterial = FileSystem.GetMaterial("bricks.vmt");
                    Mesh outputMesh = new Mesh(FileSystem.GetModelPath(propertyValue), testMaterial);
                    return outputMesh;
            }

            return null;
        }

        public static void SetProperty(object obj, string propertyName, object value)
        {
            Type type = obj.GetType();

            // Check if a PROPERTY with the given name exists
            var property = type.GetProperty(propertyName);
            if (property != null && property.CanWrite)
            {
                property.SetValue(obj, value);
                return;
            }

            // Check if a FIELD with the given name exists
            var field = type.GetField(propertyName);
            if (field != null)
            {
                field.SetValue(obj, value);
                return;
            }

            Console.WriteLine($"Property or Field '{propertyName}' not found on {type.Name}");
        }

    }
}