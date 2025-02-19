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

    }
}