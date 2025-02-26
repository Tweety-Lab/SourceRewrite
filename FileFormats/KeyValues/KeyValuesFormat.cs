using System.Globalization;
using System.Numerics;
using System.Text.RegularExpressions;


namespace FileFormats.KeyValues
{
    // This code is bad. Rewrite.

    /// <summary>
    /// Valve KeyValues file class. In Source 1, this file type is used for materials, VGUI elements, gameinfo.txt and more.
    /// </summary>
    public class KeyValuesFormat
    {
        public List<ParentKey> ParentKeys = new List<ParentKey>();
        public KeyValuesFormat(string contents)
        {
            var lines = contents.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            int currentLine = 0;

            while (currentLine < lines.Length)
            {
                var line = lines[currentLine].Trim();
                if (string.IsNullOrWhiteSpace(line) || line.StartsWith("//")) // Ignore comments and empty lines
                {
                    currentLine++;
                    continue;
                }

                var match = Regex.Match(line, @"^""?([\w$]+)""?\s*{?");
                if (match.Success)
                {
                    var parentKey = new ParentKey(match.Groups[1].Value);
                    ParentKeys.Add(parentKey);
                    currentLine = ParseBlock(lines, currentLine + 1, parentKey);
                }
                else
                {
                    currentLine++;
                }
            }
        }

        private int ParseBlock(string[] lines, int currentLine, ParentKey parentKey)
        {
            while (currentLine < lines.Length)
            {
                var line = lines[currentLine].Trim();
                if (string.IsNullOrWhiteSpace(line) || line.StartsWith("//")) // Ignore comments
                {
                    currentLine++;
                    continue;
                }

                if (line == "}") // Block end
                {
                    return currentLine + 1;
                }

                // Parse key-value pair
                var match = Regex.Match(line, @"^""?([\w$]+)""?\s+""(.+?)""$");
                if (match.Success)
                {
                    var key = match.Groups[1].Value;
                    var value = match.Groups[2].Value;

                    KeyValue keyValue = new KeyValue(key, value);
                    keyValue.Value = keyValue.ConvertValueToType(value);

                    parentKey.ChildKeyValues.Add(keyValue);
                    currentLine++;
                    continue;
                }

                // Parse nested parent key
                match = Regex.Match(line, @"^""?([\w$]+)""?\s*{?");
                if (match.Success)
                {
                    var nestedParentKey = new ParentKey(match.Groups[1].Value);
                    parentKey.ChildParentKeys.Add(nestedParentKey);
                    currentLine = ParseBlock(lines, currentLine + 1, nestedParentKey);
                    continue;
                }

                currentLine++;
            }
            return currentLine;
        }

        /// <summary>
        /// Returns the first found Parent Key with the specified name.
        /// </summary>
        public ParentKey GetParentKey(string name)
        {
            return ParentKeys.Find(pk => pk.Name == name);
        }

        /// <summary>
        /// Returns a KeyValue from its Key.
        /// </summary>
        public KeyValue GetKeyValue(string key)
        {
            foreach (ParentKey parentKey in ParentKeys)
            {
                KeyValue keyValue = parentKey.GetKeyValue(key);
                if (keyValue != null) return keyValue;
            }

            return null;
        }
    }

    /// <summary>
    /// KeyValue Class. Stored in KeyValues class.
    /// </summary>
    public class KeyValue
    {
        public string Key;
        public object Value;

        public ParentKey ParentKey; // Parent Key

        public KeyValue(string key, object value)
        {
            Value = value;
            Key = key;
        }

        /// <summary>
        /// Casts the Value to a specified Type.
        /// </summary>
        public T GetValueAsType<T>()
        {
            object result = Value; // Default to string
            string stringValue = (string)Value;

            if (typeof(T) == typeof(int) && int.TryParse(stringValue, out int intValue))
                result = intValue;
            else if (typeof(T) == typeof(float) && float.TryParse(stringValue, out float floatValue))
                result = floatValue;
            else if (typeof(T) == typeof(bool) && bool.TryParse(stringValue, out bool boolValue))
                result = boolValue;
            else if (typeof(T) == typeof(double) && double.TryParse(stringValue, out double doubleValue))
                result = doubleValue;
            else if (typeof(T) == typeof(Vector3))
            {
                string[] parts = stringValue.Split(',');
                if (parts.Length == 3 &&
                    float.TryParse(parts[0], NumberStyles.Float, CultureInfo.InvariantCulture, out float x) &&
                    float.TryParse(parts[1], NumberStyles.Float, CultureInfo.InvariantCulture, out float y) &&
                    float.TryParse(parts[2], NumberStyles.Float, CultureInfo.InvariantCulture, out float z))
                {
                    result = new Vector3(x, y, z);
                }
            }
            else if (typeof(T) == typeof(Vector4))
            {
                string[] parts = stringValue.Split(',');
                if (parts.Length == 4 &&
                    float.TryParse(parts[0], NumberStyles.Float, CultureInfo.InvariantCulture, out float x) &&
                    float.TryParse(parts[1], NumberStyles.Float, CultureInfo.InvariantCulture, out float y) &&
                    float.TryParse(parts[2], NumberStyles.Float, CultureInfo.InvariantCulture, out float z) &&
                    float.TryParse(parts[3], NumberStyles.Float, CultureInfo.InvariantCulture, out float w))
                {
                    result = new Vector4(x, y, z, w);
                }
            }

            return (T)result;
        }

        /// <summary>
        /// Convert a Value to it's type.
        /// </summary>
        public object ConvertValueToType(string input)
        {
            // Try parsing as an int
            if (int.TryParse(input, NumberStyles.Integer, CultureInfo.InvariantCulture, out int intValue))
                return intValue;

            // Try parsing as a float
            if (float.TryParse(input, NumberStyles.Float, CultureInfo.InvariantCulture, out float floatValue))
                return floatValue;

            // Try parsing as a bool
            if (bool.TryParse(input, out bool boolValue))
                return boolValue;

            // Try parsing as a vector3
            string[] parts = input.Split(',');
            if (parts.Length == 3 &&
                float.TryParse(parts[0], NumberStyles.Float, CultureInfo.InvariantCulture, out float x) &&
                float.TryParse(parts[1], NumberStyles.Float, CultureInfo.InvariantCulture, out float y) &&
                float.TryParse(parts[2], NumberStyles.Float, CultureInfo.InvariantCulture, out float z))
            {
                return new Vector3(x, y, z);
            }

            // Try parsing as a vector4
            if (parts.Length == 4 &&
                float.TryParse(parts[0], NumberStyles.Float, CultureInfo.InvariantCulture, out x) &&
                float.TryParse(parts[1], NumberStyles.Float, CultureInfo.InvariantCulture, out y) &&
                float.TryParse(parts[2], NumberStyles.Float, CultureInfo.InvariantCulture, out z) &&
                float.TryParse(parts[3], NumberStyles.Float, CultureInfo.InvariantCulture, out float w))
            {
                return new Vector4(x, y, z, w);
            }

            // If all else fails, return the original string
            return input;
        }
    }

    /// <summary>
    /// Stores KeyValues.
    /// </summary>
    public class ParentKey
    {
        public string Name { get; }
        public List<KeyValue> ChildKeyValues { get; } = new List<KeyValue>();
        public List<ParentKey> ChildParentKeys { get; } = new List<ParentKey>();

        public ParentKey(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Returns the first found Parent Key with the specified name.
        /// </summary>
        public ParentKey GetChildParentKey(string name)
        {
            return ChildParentKeys.Find(pk => pk.Name == name);
        }

        /// <summary>
        /// Returns a KeyValue inside this ParentKey from its Key.
        /// </summary>
        public KeyValue GetKeyValue(string key)
        {
            // Search KeyValues
            foreach (KeyValue keyValue in ChildKeyValues)
            {
                if (keyValue.Key == key)
                {
                    return keyValue;
                }
            }

            // Search recursively in nested parent keys
            foreach (ParentKey childParentKey in ChildParentKeys)
            {
                KeyValue keyValue = childParentKey.GetKeyValue(key);
                if (keyValue != null)
                {
                    return keyValue;
                }
            }

            return null; // Didn't find KeyValue, return null
        }
    }
}