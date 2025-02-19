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

            BSPReader reader = new BSPReader(path); // Begin reading BSP

            List<KeyValuesFormat> gameObjects = GetGameObjectKeyValues(reader);

            Console.WriteLine(gameObjects[0].GetKeyValue("position").Value);

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

    }
}