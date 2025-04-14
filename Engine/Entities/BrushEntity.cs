using SourceRewrite.AssetTypes;
using SourceRewrite.PhysicsSystem;
using SourceRewrite.Windowing.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SourceRewrite.Entities
{
    // Brush Entities are more like Source 2 "Mesh Entities" simply because we use a similar mesh vs brush approach

    /// <summary>
    /// An Entity that is tied to a brush.
    /// </summary>
    public class BrushEntity : BaseEntity
    {
        private List<Mesh> _brush;

        // Meshes that represents the bounds of the entity
        public List<Mesh> Brush
        {
            get => _brush;
            set
            {
                if (_brush != value)
                {
                    _brush = value;
                    RefreshMesh();
                }
            }
        }

        // Name Constructor
        public BrushEntity(string name) : base(name)
        {
            _brush = new List<Mesh>();
        }

        // Nameless Constructor
        public BrushEntity() : base()
        {
            _brush = new List<Mesh>();
        }

        // Render Meshes when they gets changed
        private void RefreshMesh()
        {
            foreach (Mesh mesh in Brush)
            {
                GameModules.GetModule<RenderModule>().Context.GetRendererAPI().InitMesh(mesh);
            }
        }
    }
}
