using GodotResources.Core.Serialization;

namespace GodotResources.Core.IO;

/// <summary>
/// Saves resource files.
/// </summary>
public static class ResourceSaver
{
    public static void Save(Runtime.ResourceFile file, string path)
    {
        ResourceWriter writer = new();

        string text = ResourceWriter.Write(file);

        File.WriteAllText(path, text);
    }
}
