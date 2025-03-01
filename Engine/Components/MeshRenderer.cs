using SourceRewrite.AssetTypes;
using SourceRewrite.Windowing;
using SourceRewrite.Files;
using SourceRewrite.Maps;

namespace SourceRewrite.Components
{
    /// <summary>
    /// Takes a "Mesh" Asset Type and renders it in 3D space.
    /// </summary>
    public class MeshRenderer : GameComponent
    {
        private MeshAsset _mesh;
        public MeshAsset Mesh
        {
            get => _mesh;
            set
            {
                if (_mesh != value)
                {
                    _mesh = value;
                    RefreshMesh(); // Automatically update when changed
                }
            }
        }

        /// <summary>
        /// Optional override, if set will create Mesh from this path instead of set Mesh.
        /// </summary>
        public string MeshPath;

        /// <summary>
        /// Optional override, if set will create Mesh with Mateial from this path.
        /// </summary>
        public string MaterialPath;

        public override void Start()
        {
            if (MeshPath != null)
            {
                Material material = FileSystem.GetMaterial(MaterialPath);
                Mesh = new Mesh(FileSystem.GetModelPath(MeshPath), material);
            }

            if (Mesh == null)
            {
                // If no mesh exists, make an empty one
                Mesh = new MeshAsset();
                Mesh.Vertices = [];
                Mesh.Indices = [];

                GameWindow.CurrentWindow.Renderer.InitMesh(Mesh); // Render the (empty) mesh
            }
            else
            {
                GameWindow.CurrentWindow.Renderer.InitMesh(Mesh); // Render the mesh
            }
        }

        // Render Mesh when it gets changed
        private void RefreshMesh()
        {
            if (GameWindow.CurrentWindow?.Renderer != null && _mesh != null)
            {
                GameWindow.CurrentWindow.Renderer.InitMesh(_mesh);
            }

        }
    }
}
