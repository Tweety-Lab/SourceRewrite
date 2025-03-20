using SourceRewrite.Attributes;
using SourceRewrite.Entities;
using SourceRewrite.Maps;
using SourceRewrite.Maths;
using System.Numerics;

namespace Game.Entities
{
    public class CameraController : BaseEntity
    {
        [EntityProperty("angles")]
        public Vector3 Angles;

        public override void Start()
        {
            if (CameraEntity.ActiveCamera == null)
            {
                CameraEntity camEntity = new CameraEntity();
                camEntity.Start();

                camEntity.Parent = this;

                CameraEntity.SetActiveCamera(camEntity);
            }
        }

        public override void Update()
        {
            CameraEntity.ActiveCamera.Transform.Rotation = Transform.Rotation * 10f;
        }
    }
}
