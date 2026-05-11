namespace GodotResources.Core.Runtime;

/// <summary>
/// Represents a resource section.
/// </summary>
public sealed class ResourceSection(string name)
{
    private readonly Dictionary<string, Variant> _values = [];

    public string Name { get; } = name;

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
