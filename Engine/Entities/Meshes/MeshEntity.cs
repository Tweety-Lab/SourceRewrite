using Silk.NET.Assimp;
using SourceRewrite.AssetTypes;
using SourceRewrite.Windowing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mesh = SourceRewrite.AssetTypes.Mesh;

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
        /// Constructor to create a MeshEntity with a name and optional mesh.
        /// </summary>
        public MeshEntity(string name = "MeshEntity", MeshAsset mesh = null)
            : base(name)
        {
            Mesh = mesh ?? new MeshAsset();  // If no mesh is provided, create a default mesh.
            GameWindow.CurrentWindow.Renderer.GetRendererAPI().InitMesh(Mesh);
        }

        public override void Start()
        {
            if (Mesh == null)
            {
                // If no mesh exists, make an empty one
                Mesh = new MeshAsset();
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
