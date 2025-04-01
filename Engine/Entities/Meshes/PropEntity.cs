using SourceRewrite.AssetTypes;
using SourceRewrite.Attributes;
using SourceRewrite.Files;
using SourceRewrite.Windowing;
using SourceRewrite.Windowing.Modules;

namespace SourceRewrite.Entities
{
    [Entity("prop_static")]
    public class PropEntity : MeshEntity
    {
        /// <summary>
        /// Create Mesh from this path.
        /// </summary>
        [EntityProperty("model")]
        public string MeshPath { get; set; }

        /// <summary>
        /// Get Material from this path.
        /// </summary>
        [EntityProperty("skin")]
        public string MaterialPath { get; set; }

        public override void Start()
        {
            Material material = FileSystem.GetMaterial(MaterialPath);
            Mesh = new Model(FileSystem.GetModelPath(MeshPath.Replace(".model", "")), material);

            GameModules.GetModule<RenderModule>().Context.InitMesh(Mesh); // Render the (empty) mesh
        }
    }
}
