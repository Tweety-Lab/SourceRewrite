using System;
using System.Reflection;
using System.Text;

namespace FileFormats.Binary
{
    // Handles reading and writing logic of singular binary types

    // Integers
    public class IntSerialization : BinaryType<int>
    {
        public override int FromBytes(byte[] bytes)
        {
            if (bytes.Length < 4)
                throw new ArgumentException("Byte array too short to convert to int.");

            return BitConverter.ToInt32(bytes, 0);
        }

        public override byte[] ToBytes(int value)
        {
            return BitConverter.GetBytes(value);
        }
    }

    // Floats
    public class FloatSerialization : BinaryType<float>
    {
        public override float FromBytes(byte[] bytes)
        {
            if (bytes.Length < 4)
                throw new ArgumentException("Byte array too short to convert to float.");

            return BitConverter.ToSingle(bytes, 0);
        }

        public override byte[] ToBytes(float value)
        {
            return BitConverter.GetBytes(value);
        }
    }

    // Doubles
    public class DoubleSerialization : BinaryType<double>
    {
        public override double FromBytes(byte[] bytes)
        {
            if (bytes.Length < 8)
                throw new ArgumentException("Byte array too short to convert to double.");

            return BitConverter.ToDouble(bytes, 0);
        }

        public override byte[] ToBytes(double value)
        {
            return BitConverter.GetBytes(value);
        }
    }

    // Booleans
    public class BooleanSerialization : BinaryType<bool>
    {
        public override bool FromBytes(byte[] bytes)
        {
            return BitConverter.ToBoolean(bytes, 0);
        }
        public override byte[] ToBytes(bool value)
        {
            return BitConverter.GetBytes(value);
        }
    }

    // Strings
    public class StringSerialization : BinaryType<string>
    {
        public override string FromBytes(byte[] bytes)
        {
            return Encoding.UTF8.GetString(bytes);
        }
        public override byte[] ToBytes(string value)
        {
            return Encoding.UTF8.GetBytes(value);
        }
    }

    // Structs
    // TODO: Clean up this mess
    public class StructSerialization : BinaryType<object>
    {
        public override object FromBytes(byte[] bytes)
        {
            throw new NotImplementedException("Use FromBytes(Type type, byte[] bytes) for struct deserialization");
        }

        public object FromBytes(Type type, byte[] bytes)
        {
            if (!type.IsValueType || type.IsPrimitive || type == typeof(decimal))
                throw new InvalidOperationException("Expected a non-primitive struct.");

            var instance = Activator.CreateInstance(type);
            int offset = 0;

            foreach (var field in type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                Type fieldType = field.FieldType;

                if (!BinarySerialization.BinaryTypes.TryGetValue(fieldType, out var serializer))
                {
                    // Handle nested structs
                    if (fieldType.IsValueType && !fieldType.IsPrimitive && fieldType != typeof(decimal))
                    {
                        // Estimate size by creating a default instance and serializing it
                        var tempInstance = Activator.CreateInstance(fieldType);
                        int estimatedSize = BinarySerialization.SerializeObject(tempInstance).Length;

                        if (offset + estimatedSize > bytes.Length)
                            throw new InvalidOperationException("Not enough bytes to deserialize struct field");

                        byte[] fieldBytes = new byte[estimatedSize];
                        Array.Copy(bytes, offset, fieldBytes, 0, estimatedSize);
                        offset += estimatedSize;

                        object fieldValue = BinarySerialization.DeserializeObject(fieldType, fieldBytes);
                        field.SetValue(instance, fieldValue);
                        continue;
                    }

                    throw new NotSupportedException($"Field type '{fieldType}' is not supported for deserialization.");
                }

                // Handle variable-length types (strings and arrays)
                if (fieldType == typeof(string) || fieldType.IsArray)
                {
                    if (offset + 4 > bytes.Length)
                        throw new InvalidOperationException("Not enough bytes for length prefix");

                    int length = BitConverter.ToInt32(bytes, offset);
                    offset += 4;

                    if (offset + length > bytes.Length)
                        throw new InvalidOperationException("Not enough bytes for field data");

                    byte[] fieldBytes = new byte[length];
                    Array.Copy(bytes, offset, fieldBytes, 0, length);
                    offset += length;

                    object fieldValue = serializer.FromBytes(fieldBytes);
                    field.SetValue(instance, fieldValue);
                }
                else
                {
                    // Fixed-size types
                    int size = GetSizeOfType(fieldType);
                    if (offset + size > bytes.Length)
                        throw new InvalidOperationException("Not enough bytes for field data");

                    byte[] fieldBytes = new byte[size];
                    Array.Copy(bytes, offset, fieldBytes, 0, size);
                    offset += size;

                    object fieldValue = serializer.FromBytes(fieldBytes);
                    field.SetValue(instance, fieldValue);
                }
            }

            return instance;
        }

        public override byte[] ToBytes(object value)
        {
            Type type = value.GetType();

            if (!type.IsValueType || type.IsPrimitive || type == typeof(decimal))
                throw new InvalidOperationException("Expected a non-primitive struct.");

            List<byte> result = new List<byte>();
            var fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            foreach (var field in fields)
            {
                object fieldValue = field.GetValue(value);
                byte[] fieldBytes = BinarySerialization.SerializeObject(fieldValue);

                // For variable-length types, prefix with length
                if (field.FieldType == typeof(string) || field.FieldType.IsArray)
                {
                    result.AddRange(BitConverter.GetBytes(fieldBytes.Length));
                }

                result.AddRange(fieldBytes);
            }

            return result.ToArray();
        }

        private int GetSizeOfType(Type type)
        {
            if (type == typeof(bool)) return 1;
            if (type == typeof(byte)) return 1;
            if (type == typeof(sbyte)) return 1;
            if (type == typeof(char)) return 2;
            if (type == typeof(short)) return 2;
            if (type == typeof(ushort)) return 2;
            if (type == typeof(int)) return 4;
            if (type == typeof(uint)) return 4;
            if (type == typeof(float)) return 4;
            if (type == typeof(long)) return 8;
            if (type == typeof(ulong)) return 8;
            if (type == typeof(double)) return 8;

            // For other structs, estimate size by serializing a default instance
            if (type.IsValueType && !type.IsPrimitive && type != typeof(decimal))
            {
                var temp = Activator.CreateInstance(type);
                return BinarySerialization.SerializeObject(temp).Length;
            }

            throw new NotSupportedException($"Cannot determine size of type {type}");
        }
    }
}