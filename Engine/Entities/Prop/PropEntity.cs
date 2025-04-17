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
        /// Selected Skin.
        /// </summary>
        [EntityProperty("skin")]
        public string Skin { get; set; }

        public override void Start()
        {
            Mesh = new Model(FileSystem.GetModelPath(MeshPath.Replace("models/", "")));

            GameModules.GetModule<RenderModule>().Context.InitMesh(Mesh); // Render the (empty) mesh
        }
    }
}
