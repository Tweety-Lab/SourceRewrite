using System.Globalization;
using System.Numerics;

namespace SourceFormats.KeyValues;

public static class KeyValuesUtility
{
    /// <summary>
    /// Convert a KeyValue Value to it's type.
    /// </summary>
    public static object ConvertValueToType(string input)
    {
        // Try parsing as an int
        if (int.TryParse(input, NumberStyles.Integer, CultureInfo.InvariantCulture, out var intValue))
            return intValue;

        // Try parsing as a float
        if (float.TryParse(input, NumberStyles.Float, CultureInfo.InvariantCulture, out var floatValue))
            return floatValue;

        // Try parsing as a bool
        if (bool.TryParse(input, out var boolValue))
            return boolValue;

        // Clean vector input if it has angle brackets
        var vectorInput = input;
        if (vectorInput.StartsWith("<") && vectorInput.EndsWith(">"))
            vectorInput = vectorInput.Substring(1, vectorInput.Length - 2);

        // Try parsing as a vector3
        var parts = vectorInput.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 3 &&
            float.TryParse(parts[0], NumberStyles.Float, CultureInfo.InvariantCulture, out var x) &&
            float.TryParse(parts[1], NumberStyles.Float, CultureInfo.InvariantCulture, out var y) &&
            float.TryParse(parts[2], NumberStyles.Float, CultureInfo.InvariantCulture, out var z))
            return new Vector3(x, y, z);
        // Try parsing as a vector4
        if (parts.Length == 4 &&
            float.TryParse(parts[0], NumberStyles.Float, CultureInfo.InvariantCulture, out x) &&
            float.TryParse(parts[1], NumberStyles.Float, CultureInfo.InvariantCulture, out y) &&
            float.TryParse(parts[2], NumberStyles.Float, CultureInfo.InvariantCulture, out z) &&
            float.TryParse(parts[3], NumberStyles.Float, CultureInfo.InvariantCulture, out var w))
            return new Vector4(x, y, z, w);

        // If all else fails, return the original string
        return input;
    }
}