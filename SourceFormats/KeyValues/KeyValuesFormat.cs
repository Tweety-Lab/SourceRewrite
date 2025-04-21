using System.Text.RegularExpressions;

namespace SourceFormats.KeyValues;

/// <summary>
/// Valve KeyValues file.
/// </summary>
public class KeyValuesFormat
{
    private static readonly string[] NewLineSeparators = { "\r\n", "\r", "\n" };
    private static readonly Regex ParentKeyPattern = new(@"^""?([\w$]+)""?\s*{?", RegexOptions.Compiled);
    private static readonly Regex KeyValuePattern = new(@"^\s*[""]?([^\s""]+)[""]?\s+(?:([""])(.*?)\2|(\S+))\s*$", RegexOptions.Compiled);

    /// <summary>
    /// All Parent Keys that exist in the KeyValues file at the top level.
    /// </summary>
    public List<ParentKey> ParentKeys = new();

    public KeyValuesFormat(string contents)
    {
        var lines = contents.Split(NewLineSeparators, StringSplitOptions.None);
        ParseLines(lines);
    }

    private void ParseLines(string[] lines)
    {
        for (var currentLine = 0; currentLine < lines.Length; currentLine++)
        {
            var line = lines[currentLine].Trim();

            // Ignore comments and empty lines
            if (string.IsNullOrWhiteSpace(line) || line.StartsWith("//"))
                continue;

            var parentKeyMatch = ParentKeyPattern.Match(line);
            if (parentKeyMatch.Success)
            {
                var parentKey = new ParentKey(parentKeyMatch.Groups[1].Value);
                ParentKeys.Add(parentKey);
                currentLine = ParseBlock(lines, currentLine + 1, parentKey);
            }
        }
    }

    private int ParseBlock(string[] lines, int currentLine, ParentKey parentKey)
    {
        while (currentLine < lines.Length)
        {
            var line = lines[currentLine].Trim();

            // Ignore comments and empty lines
            if (string.IsNullOrWhiteSpace(line) || line.StartsWith("//"))
            {
                currentLine++;
                continue;
            }

            if (line == "}") // Block end
                return currentLine + 1;

            var keyValueMatch = KeyValuePattern.Match(line);
            if (keyValueMatch.Success)
            {
                AddKeyValuePair(parentKey, keyValueMatch);
                currentLine++;
                continue;
            }

            var nestedParentKeyMatch = ParentKeyPattern.Match(line);
            if (nestedParentKeyMatch.Success)
            {
                currentLine = ParseNestedBlock(lines, currentLine, parentKey, nestedParentKeyMatch);
                continue;
            }

            currentLine++;
        }

        return currentLine;
    }

    /// <summary>
    /// Adds a KeyValue to the specified ParentKey from a Regex match.
    /// </summary>
    private static void AddKeyValuePair(ParentKey parentKey, Match match)
    {
        var key = match.Groups[1].Value;
        var value = match.Groups[3].Success ? match.Groups[3].Value : match.Groups[4].Value;
        var convertedValue = KeyValuesUtility.ConvertValueToType(value);

        parentKey.KeyValues.Add(new KeyValue(key, convertedValue));
    }

    /// <summary>
    /// Adds a nested ParentKey to the specified ParentKey from a Regex match.
    /// </summary>
    private int ParseNestedBlock(string[] lines, int currentLine, ParentKey parentKey, Match match)
    {
        var nestedParentKey = new ParentKey(match.Groups[1].Value);
        parentKey.ParentKeys.Add(nestedParentKey);
        return ParseBlock(lines, currentLine + 1, nestedParentKey);
    }

    /// <summary>
    /// Returns the first found Parent Key with the specified name.
    /// </summary>
    public ParentKey GetParentKey(string name)
    {
        return ParentKeys.Find(pk => pk.Name == name);
    }
}