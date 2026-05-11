namespace GodotResources.Core.Utilities;

/// <summary>
/// UID generation utility.
/// </summary>
public static class ResourceUid
{
    public static string Create()
    {
        return $"uid://{Guid.NewGuid():N}";
    }
}
