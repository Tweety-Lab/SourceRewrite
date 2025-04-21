namespace SourceFormats.KeyValues;

/// <summary>
/// Stores KeyValues.
/// </summary>
public class ParentKey
{
    public ParentKey(string name)
    {
        Name = name;
    }

    public string Name { get; }
    public List<KeyValue> KeyValues { get; } = new();
    public List<ParentKey> ParentKeys { get; } = new();

    /// <summary>
    ///     Returns the first found Parent Key with the specified name.
    /// </summary>
    public ParentKey GetChildParentKey(string name)
    {
        return ParentKeys.Find(pk => pk.Name == name);
    }

    /// <summary>
    ///     Returns a KeyValue inside this ParentKey from its Key.
    /// </summary>
    public KeyValue GetKeyValue(string key)
    {
        // Search KeyValues
        foreach (var keyValue in KeyValues)
            if (keyValue.Key == key)
                return keyValue;

        // Search recursively in nested parent keys
        foreach (var childParentKey in ParentKeys)
        {
            var keyValue = childParentKey.GetKeyValue(key);
            if (keyValue != null) return keyValue;
        }

        return null; // Didn't find KeyValue, return null
    }
}