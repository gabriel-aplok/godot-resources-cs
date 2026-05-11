namespace GodotResources.Core.Runtime;

/// <summary>
/// Represents a root resource file.
/// </summary>
public sealed class ResourceFile
{
    public string ResourceType { get; set; } = "Resource";
    public string? ScriptClass { get; set; }
    public string Uid { get; set; } = string.Empty;
    public int FormatVersion { get; set; } = 3;

    public List<ExternalResource> ExternalResources { get; } = [];
    public List<SubResource> SubResources { get; } = [];
    public List<ResourceSection> Sections { get; } = [];

    public ResourceSection Resource => Sections.First(x => x.Name == "resource");

    public SubResource? GetSubResource(string id)
    {
        return SubResources.FirstOrDefault(x => x.Id == id);
    }

    public ExternalResource? GetExternalResource(string id)
    {
        return ExternalResources.FirstOrDefault(x => x.Id == id);
    }
}
