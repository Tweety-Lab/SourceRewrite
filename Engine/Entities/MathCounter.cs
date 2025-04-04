using SourceRewrite.Attributes;
using System.Numerics;

namespace SourceRewrite.Entities
{
    [Entity("math_counter")]
    public class MathCounter : BaseEntity
    {
        [EntityProperty("startvalue")]
        public int Value { get; set; } = 0;

        [Input]
        public void Add(int amount)
        {
            Value += amount;
            DeveloperConsole.Msg($"MathCounter {Name} incremented to {Value}");
        }

        [Input]
        public void Subtract(int amount)
        {
            Value -= amount;
            DeveloperConsole.Msg($"MathCounter {Name} decremented to {Value}");
        }

        [Input]
        public void Divide(int amount)
        {
            // Avoid division by zero
            if (amount == 0)
                return;

            Value /= amount;
            DeveloperConsole.Msg($"MathCounter {Name} divided to {Value}");
        }

#if EDITOR
        public override void DrawGizmos()
        {
            Gizmos.Color = new Vector4(255, 255, 255, 0);
            Gizmos.DrawSprite(Transform.Position, "editor/math_counter", 1f, this);
        }
#endif
    }
}
