namespace GodotResources.Core.Runtime;

/// <summary>
/// Defines a custom conversion logic from a ResourceSection to a typed object.
/// </summary>
public interface IResourceConverter<T>
    where T : class, new()
{
    public T Convert(ResourceSection section);
}
