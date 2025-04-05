using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.InteropServices;

namespace FileFormats.Binary
{
    /// <summary>
    /// Binary serialization.
    /// </summary>
    public static class BinarySerialization
    {
        // Stored as <Type, IBinaryType>
        public static readonly Dictionary<Type, IBinaryType> BinaryTypes = new();

        // Register all serializable types
        static BinarySerialization()
        {
            // Singular Types
            Register<int>(new IntSerialization());
            Register<float>(new FloatSerialization());
            Register<double>(new DoubleSerialization());
            Register<bool>(new BooleanSerialization());
            Register<string>(new StringSerialization());

            Register<object>(new StructSerialization());

            // Array Types
            Register<int[]>(new IntArraySerialization());
            Register<float[]>(new FloatArraySerialization());
            Register<double[]>(new DoubleArraySerialization());
            Register<bool[]>(new BooleanArraySerialization());
            Register<string[]>(new StringArraySerialization());

            Register<object[]>(new StructArraySerialization());
        }


        public static void Register<T>(IBinaryType<T> serializer)
        {
            BinaryTypes[typeof(T)] = serializer;
        }

        // Serialize an object into a byte array
        public static byte[] SerializeObject(object obj)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));

            Type type = obj.GetType();

            if (BinaryTypes.TryGetValue(type, out var serializer))
            {
                return serializer.ToBytes(obj);
            }

            // Check if we were given a struct
            if (type.IsValueType && !type.IsPrimitive && type != typeof(decimal))
            {
                // Use the struct serializer
                var structSerializer = (IBinaryType<object>)BinaryTypes[typeof(object)];
                return structSerializer.ToBytes(obj);
            }

            throw new NotSupportedException($"Serialization for type '{type}' is not supported.");
        }


        // Serialize a byte array into an object
        public static object DeserializeObject(Type type, byte[] bytes)
        {
            if (BinaryTypes.TryGetValue(type, out var serializer))
            {
                return serializer.FromBytes(bytes);
            }

            // Check if we were given a struct
            if (type.IsValueType && !type.IsPrimitive && type != typeof(decimal))
            {
                // Use the struct serializer
                var structSerializer = (IBinaryType<object>)BinaryTypes[typeof(object)];
                return ((StructSerialization)structSerializer).FromBytes(type, bytes);
            }

            throw new NotSupportedException($"Deserialization for type '{type}' is not supported.");
        }
    }

    public interface IBinaryType
    {
        object FromBytes(byte[] bytes);
        byte[] ToBytes(object value);
    }

    public interface IBinaryType<T> : IBinaryType
    {
        new T FromBytes(byte[] bytes);
        byte[] ToBytes(T value);
    }

    // For Single Types
    public abstract class BinaryType<T> : IBinaryType<T>
    {
        public abstract T FromBytes(byte[] bytes);
        public abstract byte[] ToBytes(T value);

        object IBinaryType.FromBytes(byte[] bytes) => FromBytes(bytes);
        byte[] IBinaryType.ToBytes(object value) => ToBytes((T)value);
    }

    // For Array Types
    public abstract class BinaryArrayType<T> : BinaryType<T[]>
    {
        protected abstract BinaryType<T> ElementSerializer { get; }

        public override byte[] ToBytes(T[] value)
        {
            List<byte> bytes = new List<byte>();

            // Write array length
            bytes.AddRange(BitConverter.GetBytes(value.Length));

            // Write each element
            foreach (T item in value)
            {
                var itemBytes = ElementSerializer.ToBytes(item);
                // For variable-length types (like strings), prefix with length
                if (IsVariableLength(itemBytes))
                {
                    bytes.AddRange(BitConverter.GetBytes(itemBytes.Length));
                }
                bytes.AddRange(itemBytes);
            }

            return bytes.ToArray();
        }

        public override T[] FromBytes(byte[] bytes)
        {
            int offset = 0;

            int length = BitConverter.ToInt32(bytes, offset);
            offset += 4;

            T[] result = new T[length];

            for (int i = 0; i < length; i++)
            {
                byte[] itemBytes;

                if (UsesVariableLength())
                {
                    int itemLength = BitConverter.ToInt32(bytes, offset);
                    offset += 4;

                    itemBytes = new byte[itemLength];
                    Array.Copy(bytes, offset, itemBytes, 0, itemLength);
                    offset += itemLength;
                }
                else
                {
                    int fixedSize = ElementSize();
                    itemBytes = new byte[fixedSize];
                    Array.Copy(bytes, offset, itemBytes, 0, fixedSize);
                    offset += fixedSize;
                }

                result[i] = ElementSerializer.FromBytes(itemBytes);
            }

            return result;
        }

        protected virtual bool UsesVariableLength() => false;
        protected virtual int ElementSize() => throw new NotImplementedException();

        private bool IsVariableLength(byte[] itemBytes) => UsesVariableLength();
    }
}