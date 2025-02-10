using SourceRewrite.Components;
using Silk.NET;
using System.Reflection;
using SourceRewrite.Windowing;
using SourceRewrite.Objects;

namespace Game
{
    public class TestComponent : GameComponent
    {
        public override void Start()
        {
            Console.WriteLine("Logic Called From Game!");
        }
    }
}
