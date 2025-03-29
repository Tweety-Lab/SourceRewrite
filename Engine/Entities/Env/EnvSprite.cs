using SourceRewrite.AssetTypes;
using SourceRewrite.Attributes;
using SourceRewrite.Files;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SourceRewrite.Entities.Env
{
    [AlwaysExecute]
    [Entity("env_sprite")]
    public class EnvSprite : MeshEntity
    {
        [EntityProperty("model")]
        public string SpriteName { get; set; }

        public override void Start()
        {
            Mesh newMesh = new Mesh();
            newMesh.Material = FileSystem.GetMaterial(SpriteName.Replace(".vmt", ""));

            // Placeholder Scaling factor
            float scaleFactor = 20.0f;

            // Create a scaled Quad Mesh
            newMesh.Vertices = new float[] {
    -0.5f * scaleFactor,  0.5f * scaleFactor, 0,   // Vertex 1
     0.5f * scaleFactor,  0.5f * scaleFactor, 0,   // Vertex 2
     0.5f * scaleFactor, -0.5f * scaleFactor, 0,   // Vertex 3
    -0.5f * scaleFactor, -0.5f * scaleFactor, 0    // Vertex 4
};

            newMesh.Indices = new uint[] { 0, 1, 2, 0, 2, 3 };

            Mesh = newMesh;
        }
    }
}
