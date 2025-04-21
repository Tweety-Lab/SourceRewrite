namespace SourceFormats.KeyValues;

/// <summary>
/// Valve KeyValue.
/// </summary>
public class KeyValue
{
    public string Key;
    public object Value;

    public KeyValue(string key, object value)
    {
        Value = value;
        Key = key;
    }
}