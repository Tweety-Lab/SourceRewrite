using FileFormats.BSP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SourceRewrite.AssetTypes;
using SourceRewrite.Components;
using SourceRewrite.Objects;
using SourceRewrite.Files;

namespace SourceRewrite.Maps
{
    // BSP Map System
    public static class MapSystem
    {
        public static void LoadMap(string path)
        {
            // BSP Exists, we can read it
            if (File.Exists(path))
            {

                // Render the brush geometry with a BSPMesh and MeshRenderer
                BSPMesh bspMesh = new BSPMesh(path);
                MeshRenderer bspRenderer = new MeshRenderer(bspMesh.Mesh);

                // Create a GameObject to hold BSP Geometry
                GameObject BspGeometry = new GameObject();
                BspGeometry.AddComponent(bspRenderer);

                // Create Camera
                GameObject cameraObject = new GameObject();
                cameraObject.AddComponent(new Camera());
                cameraObject.AddComponent(new CameraController());


                ///////////////////////////
                // READ DATA FROM BSP
                ///////////////////////////
                BSPReader reader = new BSPReader(path);

                string mapComponents = reader.GetLumpData<string>(LumpType.LUMP_MAP_COMPONENTS);

                Material meshRendererMaterial = FileSystem.GetMaterial("bricks.vmt");
                Mesh meshRendererMesh = new Mesh(FileSystem.GetModelPath("cube.model"), meshRendererMaterial);

                // Constructor arguments
                object[] constructorArgs = new object[] { meshRendererMesh };

                var componentCreator = new ComponentCreator();

                componentCreator.CreateClassFromNamespace(mapComponents, constructorArgs);
            }
        }
    }

    // Create components from a namespace string
    public class ComponentCreator
    {
        public object CreateClassFromNamespace(string fullClassName, object[] constructorArguments)
        {
            // Get the Type object of the class using its full name
            Type classType = Type.GetType(fullClassName);

            if (classType == null)
            {
                throw new InvalidOperationException($"Class with name {fullClassName} not found.");
            }

            // Create an instance of the class using the constructor that matches the arguments
            return Activator.CreateInstance(classType, constructorArguments);
        }
    }
}

