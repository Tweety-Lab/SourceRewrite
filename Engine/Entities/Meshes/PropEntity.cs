using SourceRewrite.AssetTypes;
using SourceRewrite.Attributes;
using SourceRewrite.Files;
using SourceRewrite.Windowing;

namespace SourceRewrite.Entities
{
    [Entity("prop_static")]
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
