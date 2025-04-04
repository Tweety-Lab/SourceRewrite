using SourceRewrite.AssetTypes;
using SourceRewrite.Attributes;
using SourceRewrite.Files;

namespace SourceRewrite.Entities.Env
{
    [AlwaysExecute]
    [Entity("env_sprite")]
    public class EnvSprite : MeshEntity
    {
        [EntityProperty("model")]
        public string SpriteName { get; set; }

        [EntityProperty("scale")]
        public float ScaleFactor { get; set; } = 1.0f;

        public override void Start()
        {
            Mesh newMesh = new Mesh();
            newMesh.Material = FileSystem.GetMaterial(SpriteName.Replace(".vmt", ""));

            ScaleFactor *= 20f; // Scale the sprite

            // Create a scaled Quad Mesh
            newMesh.Vertices = new float[] {
    -0.5f * ScaleFactor,  0.5f * ScaleFactor, 0,   // Vertex 1
     0.5f * ScaleFactor,  0.5f * ScaleFactor, 0,   // Vertex 2
     0.5f * ScaleFactor, -0.5f * ScaleFactor, 0,   // Vertex 3
    -0.5f * ScaleFactor, -0.5f * ScaleFactor, 0    // Vertex 4
};

            newMesh.Indices = new uint[] { 0, 1, 2, 0, 2, 3 };

            Mesh = newMesh;
        }
    }
}
