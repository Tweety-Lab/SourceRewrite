using SourceRewrite.Attributes;
using System.Numerics;

namespace SourceRewrite.Entities
{
    [Entity("math_counter")]
    public class MathCounter : PointEntity
    {

        private int _value;

        [EntityProperty("startvalue")]
        public int Value
        {
            get => _value;
            set
            {
                int clampedValue = value;

                // Clamp to MinValue only if its is non-zero
                if (MinValue != 0 && clampedValue < MinValue)
                    clampedValue = MinValue;

                // Clamp to MaxValue only if its is non-zero
                if (MaxValue != 0 && clampedValue > MaxValue)
                    clampedValue = MaxValue;

                // Only continue if the value is actually changing
                if (_value == clampedValue)
                    return;

                _value = clampedValue;

                // Check for hitting the minimum
                if (MinValue != 0 && _value == MinValue)
                {
                    FireOutput("OnHitMin");
                }

                // Check for hitting the maximum
                if (MaxValue != 0 && _value == MaxValue)
                {
                    FireOutput("OnHitMax");
                }
            }
        }

        [EntityProperty("min")]
        public int MinValue { get; set; }

        [EntityProperty("max")]
        public int MaxValue { get; set; }

        [Input]
        public void Add(int amount)
        {
            Value += amount;
        }

        [Input]
        public void Subtract(int amount)
        {
            Value -= amount;
        }

        [Input]
        public void Divide(int amount)
        {
            // Avoid division by zero
            if (amount == 0)
                return;

            Value /= amount;
        }

        [Input]
        public void Multiply(int amount)
        {
            Value *= amount;
        }

        [Input]
        public void SetValue(int amount)
        {
            Value = amount;
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
