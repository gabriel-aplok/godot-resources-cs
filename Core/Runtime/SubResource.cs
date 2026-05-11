namespace GodotResources.Core.Runtime;

/// <summary>
/// Represents a sub-resource defined within a resource file.
/// </summary>
public sealed class SubResource
{
    public string Type { get; set; } = string.Empty;
    public string Id { get; set; } = string.Empty;
    private readonly Dictionary<string, Variant> _values = [];

    public SubResource() { }

    public SubResource(string id)
    {
        Id = id;
    }

    public IReadOnlyDictionary<string, Variant> Values => _values;

    public Variant this[string key]
    {
        get => _values[key];
        set => _values[key] = value;
    }

    public T Get<T>(string key)
    {
        return _values[key].Get<T>();
    }

    public bool TryGetValue(string key, out Variant value)
    {
        return _values.TryGetValue(key, out value!);
    }
}
