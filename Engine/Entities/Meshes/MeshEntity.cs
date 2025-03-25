using SourceRewrite.Windowing;
using Mesh = SourceRewrite.AssetTypes.Model;

namespace SourceRewrite.Entities
{
    /// <summary>
    /// Takes a "Mesh" Asset Type and renders it in 3D space.
    /// </summary>
    public class MeshEntity : BaseEntity
    {
        /// <summary>
        /// The mesh associated with the entity.
        /// </summary>
        private AssetTypes.Mesh _mesh;
        public AssetTypes.Mesh Mesh
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
        /// Constructor to create a MeshEntity with a name and optional mesh.
        /// </summary>
        public MeshEntity(string name = "MeshEntity", AssetTypes.Mesh mesh = null)
            : base(name)
        {
            Mesh = mesh ?? new AssetTypes.Mesh();  // If no mesh is provided, create a default mesh.
            GameWindow.CurrentWindow.Renderer.GetRendererAPI().InitMesh(Mesh);
        }

        public override void Start()
        {
            if (Mesh == null)
            {
                // If no mesh exists, make an empty one
                Mesh = new AssetTypes.Mesh();
                Mesh.Vertices = [];
                Mesh.Indices = [];

                GameWindow.CurrentWindow.Renderer.InitMesh(Mesh); // Render the (empty) mesh
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
