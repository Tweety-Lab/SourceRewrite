using SourceRewrite.Entities;
using SourceRewrite.Maps;
using SourceRewrite.Maths;
using System.Numerics;

namespace Game.Entities
{
    public class CameraController : BaseEntity
    {
        [MapProperty("angles")]
        public Vector3 Angles;

        public override void Start()
        {
            Console.WriteLine("Starting Camera Controller");

            Console.WriteLine(Angles.ToString());
        }

        public override void Update()
        {
            Console.WriteLine("Updating Camera Controller");
        }
    }
}
