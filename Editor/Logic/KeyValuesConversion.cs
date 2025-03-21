using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Logic
{
    public static class KeyValuesConversion
    {
        // Conversion from KeyValues to C#
        private static Dictionary<Type, Func<string, object>> typeConverters;

        static KeyValuesConversion()
        {
            InitializeTypeConverters();
        }

        private static void InitializeTypeConverters()
        {
            typeConverters = new Dictionary<Type, Func<string, object>>
            {
                { typeof(int), s => int.Parse(s) },
                { typeof(float), s => float.Parse(s) },
                { typeof(bool), s => bool.Parse(s) },
                { typeof(string), s => s },
                { typeof(double), s => double.Parse(s) },
                { typeof(decimal), s => decimal.Parse(s) },
                { typeof(long), s => long.Parse(s) },
                { typeof(DateTime), s => DateTime.Parse(s) },
                { typeof(System.Numerics.Vector4), ParseVector4 }
            };
        }

        private static object ParseVector4(string input)
        {
            input = input.Trim('<', '>');
            string[] components = input.Split(',');

            if (components.Length == 4)
            {
                float x = float.Parse(components[0].Trim());
                float y = float.Parse(components[1].Trim());
                float z = float.Parse(components[2].Trim());
                float w = float.Parse(components[3].Trim());

                return new System.Numerics.Vector4(x, y, z, w);
            }

            throw new FormatException($"Invalid format for Vector4: {input}");
        }

        public static object ConvertToFieldType(string inputValue, Type fieldType)
        {
            try
            {
                if (typeConverters.TryGetValue(fieldType, out var converter))
                {
                    return converter(inputValue);
                }

                Console.WriteLine($"Unsupported field type {fieldType}");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error converting '{inputValue}' to {fieldType}: {ex.Message}");
                return null;
            }
        }
    }
}
