using SourceRewrite.AssetTypes;
using SourceRewrite.Attributes;
using SourceRewrite.Files;
using SourceRewrite.Maps;
using SourceRewrite.Windowing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SourceRewrite.Entities
{
    public class PropEntity : MeshEntity
    {
        /// <summary>
        /// Create Mesh from this path.
        /// </summary>
        [EntityProperty("model")]
        public string MeshPath;

        /// <summary>
        /// Get Material from this path.
        /// </summary>
        [EntityProperty("skin")]
        public string MaterialPath;

        public override void Start()
        {
            Material material = FileSystem.GetMaterial(MaterialPath);
            Mesh = new Model(FileSystem.GetModelPath(MeshPath), material);

            GameWindow.CurrentWindow.Renderer.InitMesh(Mesh); // Render the (empty) mesh
        }
    }
}
