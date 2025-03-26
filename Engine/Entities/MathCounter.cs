using SourceRewrite.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SourceRewrite.Entities
{
    [Entity("math_counter")]
    public class MathCounter : BaseEntity
    {
        [EntityProperty("startvalue")]
        public int Value { get; set; } = 0;

        [Input]
        public void Add()
        {
            Value++;
            DeveloperConsole.Msg($"MathCounter {Name} incremented to {Value}");
        }

        [Input]
        public void Subtract()
        {
            Value--;
            DeveloperConsole.Msg($"MathCounter {Name} decremented to {Value}");
        }
    }
}
