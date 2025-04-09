using BulletSharp;
using SourceRewrite.AssetTypes;
using SourceRewrite.Attributes;
using SourceRewrite.Files;
using SourceRewrite.PhysicsSystem;
using SourceRewrite.Windowing.Modules;

namespace SourceRewrite.Entities
{
    /// <summary>
    /// Base Entity for all Prop Entities.
    /// </summary>
    [AlwaysExecute]
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
            Mesh = new Model(FileSystem.GetModelPath(MeshPath.Replace("models/", "")), material);

            GameModules.GetModule<RenderModule>().Context.InitMesh(Mesh); // Render the (empty) mesh
        }
    }
}
