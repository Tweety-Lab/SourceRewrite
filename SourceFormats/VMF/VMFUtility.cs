using System.Globalization;
using System.Numerics;
using SourceFormats.KeyValues;

namespace SourceFormats.VMF;

public static class VMFUtility
{
    /// <summary>
    /// Converts a VMF Side Plane into 3 Vector3s.
    /// </summary>
    public static Vector3[] ProcessSidePlane(string input)
    {
        // Remove any extra whitespace and split by closing and opening parentheses
        var parts = input
            .Replace("(", "") // Remove opening parentheses
            .Replace(")", "") // Remove closing parentheses
            .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries) // Split by spaces and ignore empty entries
            .Select(s => float.Parse(s, CultureInfo.InvariantCulture)) // Parse to float
            .ToArray();

        // Ensure that we have exactly 9 values (3 sets of 3 values)
        if (parts.Length != 9)
        {
            Console.WriteLine($"Error: Expected 9 values but got {parts.Length}. Input: '{input}'");
            return new Vector3[0]; // Return empty array to handle error gracefully
        }

        // Return the three Vector3s (each with 3 components)
        return new[]
        {
            new Vector3(parts[0], parts[1], parts[2]), // First Vector3
            new Vector3(parts[3], parts[4], parts[5]), // Second Vector3
            new Vector3(parts[6], parts[7], parts[8]) // Third Vector3
        };
    }

    // Format: "[1 0 0 0] 0.25" where:
    // - [1 0 0 0] is the Vector4 components
    // - 0.25 is the scale value
    /// <summary>
    /// Converts a keyvalue string to a VMFUVAxis.
    /// </summary>
    public static VMFUVAxis ProcessUVAxis(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return new VMFUVAxis
            {
                Vector = Vector4.Zero,
                Scale = 1.0f
            };

        try
        {
            // Split into vector and scale parts
            var parts = input.Split(new[] { ']' }, 2, StringSplitOptions.RemoveEmptyEntries);

            // Parse vector components (remove the '[' and split components)
            var vectorComponents = parts[0].TrimStart('[').Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            var vector = new Vector4(
                float.Parse(vectorComponents[0], CultureInfo.InvariantCulture),
                float.Parse(vectorComponents[1], CultureInfo.InvariantCulture),
                float.Parse(vectorComponents[2], CultureInfo.InvariantCulture),
                float.Parse(vectorComponents[3], CultureInfo.InvariantCulture)
            );

            // Parse scale value
            var scale = float.Parse(parts[1].Trim(), CultureInfo.InvariantCulture);

            return new VMFUVAxis
            {
                Vector = vector,
                Scale = scale
            };
        }
        catch (Exception ex)
        {
            // Log error
            Console.WriteLine($"Error parsing UV axis '{input}': {ex.Message}");

            // Return default values on failure
            return new VMFUVAxis
            {
                Vector = Vector4.Zero,
                Scale = 1.0f
            };
        }
    }
}