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

        public override byte[] ToBytes(object[] value)
        {
            List<byte> bytes = new List<byte>();

            // Write array length
            bytes.AddRange(BitConverter.GetBytes(value.Length));

            // Write type information for the array elements (TODO: Optimize this so it uses less bytes)
            if (value.Length > 0)
            {
                Type elementType = value[0].GetType();
                string typeName = elementType.AssemblyQualifiedName;
                byte[] typeNameBytes = Encoding.UTF8.GetBytes(typeName);
                bytes.AddRange(BitConverter.GetBytes(typeNameBytes.Length));
                bytes.AddRange(typeNameBytes);
            }
            else
            {
                // Handle empty array case
                bytes.AddRange(BitConverter.GetBytes(0)); // Zero length type name
            }

            // Write each element
            foreach (object item in value)
            {
                var itemBytes = _serializer.ToBytes(item);
                // Always prefix with length for structs as they can be variable length
                bytes.AddRange(BitConverter.GetBytes(itemBytes.Length));
                bytes.AddRange(itemBytes);
            }

            return bytes.ToArray();
        }

        public override object[] FromBytes(byte[] bytes)
        {
            int offset = 0;

            // Read array length
            int length = BitConverter.ToInt32(bytes, offset);
            offset += 4;

            // Read type information
            int typeNameLength = BitConverter.ToInt32(bytes, offset);
            offset += 4;

            Type elementType = null;
            if (typeNameLength > 0)
            {
                byte[] typeNameBytes = new byte[typeNameLength];
                Array.Copy(bytes, offset, typeNameBytes, 0, typeNameLength);
                offset += typeNameLength;

                string typeName = Encoding.UTF8.GetString(typeNameBytes);
                elementType = Type.GetType(typeName);

                if (elementType == null)
                    throw new InvalidOperationException($"Could not resolve type: {typeName}");
            }
            else if (length > 0)
            {
                throw new InvalidOperationException("Array has elements but no type information");
            }

            object[] result = new object[length];

            for (int i = 0; i < length; i++)
            {
                // Read element length
                int itemLength = BitConverter.ToInt32(bytes, offset);
                offset += 4;

                // Read element bytes
                byte[] itemBytes = new byte[itemLength];
                Array.Copy(bytes, offset, itemBytes, 0, itemLength);
                offset += itemLength;

                // Deserialize the element using the StructSerialization with the known type
                result[i] = elementType != null
                    ? ((StructSerialization)_serializer).FromBytes(elementType, itemBytes)
                    : null;
            }

            return result;
        }
    }
}
