using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileFormats.Binary
{
    // Handles reading and writing logic of array/list binary types

    // Int Array
    public class IntArraySerialization : BinaryArrayType<int>
    {
        private static readonly IntSerialization _serializer = new IntSerialization();
        protected override BinaryType<int> ElementSerializer => _serializer;
        protected override int ElementSize() => 4;
    }

    // Float Array
    public class FloatArraySerialization : BinaryArrayType<float>
    {
        private static readonly FloatSerialization _serializer = new FloatSerialization();
        protected override BinaryType<float> ElementSerializer => _serializer;
        protected override int ElementSize() => 4;
    }

    // Double Array
    public class DoubleArraySerialization : BinaryArrayType<double>
    {
        private static readonly DoubleSerialization _serializer = new DoubleSerialization();
        protected override BinaryType<double> ElementSerializer => _serializer;
        protected override int ElementSize() => 8;
    }

    // Boolean Array
    public class BooleanArraySerialization : BinaryArrayType<bool>
    {
        private static readonly BooleanSerialization _serializer = new BooleanSerialization();
        protected override BinaryType<bool> ElementSerializer => _serializer;
        protected override int ElementSize() => 1;
    }

    // String Array
    public class StringArraySerialization : BinaryArrayType<string>
    {
        private static readonly StringSerialization _serializer = new StringSerialization();
        protected override BinaryType<string> ElementSerializer => _serializer;
        protected override bool UsesVariableLength() => true;
    }

    // Struct Array
    public class StructArraySerialization : BinaryArrayType<object>
    {
        private static readonly StructSerialization _serializer = new StructSerialization();
        protected override BinaryType<object> ElementSerializer => _serializer;
        protected override bool UsesVariableLength() => true;
    }
}
