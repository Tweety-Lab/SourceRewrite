using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;


namespace FileFormats.KeyValues
{
    // This code is bad. Rewrite.

    /// <summary>
    /// Valve KeyValues file class. In Source 1, this file type is used for materials, VGUI elements, gameinfo.txt and more.
    /// </summary>
    public class KeyValuesFormat
    {
        public List<ParentKey> ParentKeys = new List<ParentKey>();
        public KeyValuesFormat(string input)
        {
            var lines = input.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            int currentLine = 0;

            while (currentLine < lines.Length)
            {
                var line = lines[currentLine].Trim();

                if (string.IsNullOrWhiteSpace(line))
                {
                    currentLine++;
                    continue;
                }

                // Parse parent key
                var match = Regex.Match(line, @"([\w$]+)\s*{?");
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

                if (string.IsNullOrWhiteSpace(line))
                {
                    currentLine++;
                    continue;
                }

                // Check if we're at the end of a block
                if (line == "}")
                {
                    return currentLine + 1;
                }

                // Parse key-value pair
                var match = Regex.Match(line, @"([\w$]+)\s+""(.+)""");
                if (match.Success)
                {
                    var key = match.Groups[1].Value;
                    var value = match.Groups[2].Value;

                    var trueValue = ConvertValueToType(value); // Convert the string to it's actual type

                    var keyValue = new KeyValue(key, trueValue)
                    {
                        ParentKey = parentKey
                    };
                    parentKey.ChildKeys.Add(keyValue);
                    currentLine++;
                }
                // Check for nested block
                else
                {
                    match = Regex.Match(line, @"(\w+)\s*{");
                    if (match.Success)
                    {
                        var nestedParentKey = new ParentKey(match.Groups[1].Value);
                        ParentKeys.Add(nestedParentKey);
                        currentLine = ParseBlock(lines, currentLine + 1, nestedParentKey);
                    }
                    else
                    {
                        currentLine++;
                    }
                }
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

        /// <summary>
        /// Convert a Value to it's type.
        /// </summary>
        private object ConvertValueToType(string input)
        {
            // Try parsing as an int
            if (int.TryParse(input, NumberStyles.Integer, CultureInfo.InvariantCulture, out int intValue))
                return intValue;

            // Try parsing as a float
            if (float.TryParse(input, NumberStyles.Float, CultureInfo.InvariantCulture, out float floatValue))
                return floatValue;

            // If all else fails, return the original string
            return input;
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

            return (T)result;
        }
    }

    /// <summary>
    /// Stores KeyValues.
    /// </summary>
    public class ParentKey
    {
        public List<KeyValue> ChildKeys = new List<KeyValue>();
        public string Name;

        public ParentKey(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Returns a KeyValue inside this ParentKey from its Key.
        /// </summary>
        public KeyValue GetKeyValue(string key)
        {
            foreach (KeyValue keyValue in ChildKeys)
            {
                if (keyValue.Key == key)
                {
                    return keyValue;
                }
            }

            return null; // Didn't find KeyValue, return null
        }
    }
}