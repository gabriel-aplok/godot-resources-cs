namespace GodotResources.Core.Runtime;

/// <summary>
/// Represents an external resource.
/// </summary>
public sealed class ExternalResource
{
    public string? Id { get; set; }
    public string? Type { get; set; }
    public string? Path { get; set; }
    public string? Uid { get; set; }
}
