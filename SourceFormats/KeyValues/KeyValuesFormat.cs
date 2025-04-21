using System.Text.RegularExpressions;

namespace SourceFormats.KeyValues;

/// <summary>
/// Valve KeyValues file class. In Source 1, this file type is used for materials, VGUI elements, gameinfo.txt and
/// more.
/// </summary>
public class KeyValuesFormat
{
    public List<ParentKey> ParentKeys = new();

    public KeyValuesFormat(string contents)
    {
        var lines = contents.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
        var currentLine = 0;

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
                return currentLine + 1;

            // Parse key-value pair
            var match = Regex.Match(line, @"^\s*[""]?([^\s""]+)[""]?\s+(?:([""])(.*?)\2|(\S+))\s*$");

            if (match.Success)
            {
                var key = match.Groups[1].Value;
                var value = match.Groups[3].Success ? match.Groups[3].Value : match.Groups[4].Value;

                var keyValue = new KeyValue(key, value);
                keyValue.Value = KeyValuesUtility.ConvertValueToType(value);

                parentKey.KeyValues.Add(keyValue);
                currentLine++;
                continue;
            }

            // Parse nested parent key
            match = Regex.Match(line, @"^""?([\w$]+)""?\s*{?");
            if (match.Success)
            {
                var nestedParentKey = new ParentKey(match.Groups[1].Value);
                parentKey.ParentKeys.Add(nestedParentKey);
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
        foreach (var parentKey in ParentKeys)
        {
            var keyValue = parentKey.GetKeyValue(key);
            if (keyValue != null) return keyValue;
        }

        return null;
    }
}