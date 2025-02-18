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

namespace SourceRewrite.Maps
{
    public static class MapSystem
    {
        public static void LoadMap(string path)
        {
            if (!File.Exists(path))
                return;

            CreateBspGeometry(path);
            CreateCamera();
            ProcessBspData(path);
        }

        private static void CreateBspGeometry(string path)
        {
            var bspMesh = new BSPMesh(path);
            var bspRenderer = new MeshRenderer(bspMesh.Mesh);

            var bspGeometry = new GameObject();
            bspGeometry.AddComponent(bspRenderer);
        }

        private static void CreateCamera()
        {
            var cameraObject = new GameObject();
            cameraObject.AddComponent(new Camera());
            cameraObject.AddComponent(new CameraController());
        }

        private static void ProcessBspData(string path)
        {
            var reader = new BSPReader(path);
            var mapComponents = reader.GetLumpData<string>(LumpType.LUMP_MAP_COMPONENTS);

            var material = FileSystem.GetMaterial("bricks.vmt");
            var mesh = new Mesh(FileSystem.GetModelPath("cube.model"), material);

            ParseAndCreateComponent(mapComponents, mesh);
        }

        private static void ParseAndCreateComponent(string mapComponents, Mesh mesh)
        {
            // Input format: "Namespace.Namespace(0, 0, 0)"
            var openParenIndex = mapComponents.IndexOf('(');
            var closeParenIndex = mapComponents.IndexOf(')');

            if (openParenIndex == -1 || closeParenIndex == -1)
            {
                throw new FormatException($"Invalid component format: {mapComponents}");
            }

            // Get namespace part before the parentheses
            var nameSpace = mapComponents.Substring(0, openParenIndex).Trim();

            // Get the coordinates between the parentheses
            var transformString = mapComponents.Substring(
                openParenIndex + 1,
                closeParenIndex - openParenIndex - 1
            );

            // Parse the vector components
            var transformValues = transformString.Split(',')
                .Select(x => float.Parse(x.Trim()))
                .ToArray();

            if (transformValues.Length != 3)
            {
                throw new FormatException($"Expected 3 coordinates, got {transformValues.Length}");
            }

            var position = new Vector3(
                transformValues[0],
                transformValues[1],
                transformValues[2]  // Fixed: Using index 2 for z-coordinate
            );

            var componentCreator = new ComponentCreator();
            var component = (GameComponent)componentCreator.CreateClassFromNamespace(
                nameSpace,
                new object[] { mesh }
            );

            var gameObject = new GameObject();
            gameObject.AddComponent(component);
            gameObject.Transform.Position = position;
        }
    }

    public class ComponentCreator
    {
        public object CreateClassFromNamespace(string fullClassName, object[] constructorArguments)
        {
            Type classType = Type.GetType(fullClassName);

            if (classType == null)
            {
                throw new InvalidOperationException($"Class with name {fullClassName} not found.");
            }

            return Activator.CreateInstance(classType, constructorArguments);
        }
    }
}