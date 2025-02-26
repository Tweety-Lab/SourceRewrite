using SourceRewrite.Components;

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
