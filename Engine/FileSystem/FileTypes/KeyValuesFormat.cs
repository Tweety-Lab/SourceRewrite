using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;


namespace SourceRewrite.FileSystem.FileTypes
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
                var match = Regex.Match(line, @"(\w+)\s*{?");
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
                var match = Regex.Match(line, @"(\w+)\s+""(.+)""");
                if (match.Success)
                {
                    var key = match.Groups[1].Value;
                    var value = match.Groups[2].Value;

                    var keyValue = new KeyValue(key, value)
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
            foreach (ParentKey parentKey in ParentKeys)
            {
                if (parentKey.Name == name)
                {
                    return parentKey;
                }
            }
            return null;
        }

        /// <summary>
        /// Returns a KeyValue from its Key.
        /// </summary>
        public KeyValue GetKeyValue(string key)
        {
            foreach (ParentKey parentKey in ParentKeys)
            {
                foreach (KeyValue keyValue in parentKey.ChildKeys)
                {
                    if (keyValue.Key == key) {
                        return keyValue;
                    }
                }
            }
            return null; // Didn't find KeyValue, return null
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
