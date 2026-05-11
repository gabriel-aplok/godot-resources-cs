using System.Globalization;
using System.Text;
using GodotResources.Core.Runtime;

namespace GodotResources.Core.Serialization;

/// <summary>
/// Serializes resource files.
/// </summary>
public sealed class ResourceWriter
{
    public static string Write(ResourceFile file)
    {
        StringBuilder sb = new();

        // header
        sb.Append($"[gd_resource type=\"{file.ResourceType}\" ");
        if (!string.IsNullOrEmpty(file.ScriptClass))
        {
            sb.Append($"script_class=\"{file.ScriptClass}\" ");
        }

        sb.AppendLine($"format={file.FormatVersion} uid=\"{file.Uid}\"]");
        sb.AppendLine();

        // external resources
        foreach (ExternalResource ext in file.ExternalResources)
        {
            sb.AppendLine(
                $"[ext_resource type=\"{ext.Type}\" uid=\"{ext.Uid}\" path=\"{ext.Path}\" id=\"{ext.Id}\"]"
            );
            sb.AppendLine();
        }

        // sub resources
        foreach (SubResource sub in file.SubResources)
        {
            sb.Append("[sub_resource ");
            if (!string.IsNullOrEmpty(sub.Type))
            {
                sb.Append($"type=\"{sub.Type}\" ");
            }

            sb.AppendLine($"id=\"{sub.Id}\"]");

            foreach (KeyValuePair<string, Variant> pair in sub.Values)
            {
                sb.AppendLine($"{pair.Key} = {WriteVariant(pair.Value.Value)}");
            }
            sb.AppendLine();
        }

        foreach (ResourceSection section in file.Sections)
        {
            sb.AppendLine($"[{section.Name}]");
            foreach (KeyValuePair<string, Variant> pair in section.Values)
            {
                sb.AppendLine($"{pair.Key} = {WriteVariant(pair.Value.Value)}");
            }
            sb.AppendLine();
        }

        return sb.ToString();
    }

    private static string WriteVariant(object? value)
    {
        return value switch
        {
            null => "null",
            string s => $"\"{Escape(s)}\"",
            bool b => b ? "true" : "false",
            int i => i.ToString(),
            float f => f.ToString(CultureInfo.InvariantCulture),
            double d => d.ToString(CultureInfo.InvariantCulture),
            ExtResourceReference ext => $"ExtResource(\"{ext.Id}\")",
            SubResourceReference sub => $"SubResource(\"{sub.Id}\")",
            // Add support for Dictionaries
            System.Collections.IDictionary dict => WriteDictionary(dict),

            // Add support for Lists/Arrays (must be after String check)
            System.Collections.IEnumerable enumerable => WriteArray(enumerable),
            _ => value.ToString() ?? "null",
        };
    }

    private static string Escape(string value)
    {
        return value.Replace("\\", "\\\\").Replace("\"", "\\\"");
    }

    private static string WriteArray(System.Collections.IEnumerable list)
    {
        List<string> items = [];
        foreach (object? item in list)
        {
            // recursively write items (handles nested lists/dicts)
            items.Add(WriteVariant(item is Variant v ? v.Value : item));
        }
        return $"[{string.Join(", ", items)}]";
    }

    private static string WriteDictionary(System.Collections.IDictionary dict)
    {
        List<string> pairs = [];
        foreach (System.Collections.DictionaryEntry entry in dict)
        {
            string key = WriteVariant(entry.Key is Variant vk ? vk.Value : entry.Key);
            string val = WriteVariant(entry.Value is Variant vv ? vv.Value : entry.Value);
            pairs.Add($"{key}: {val}");
        }
        return $"{{{string.Join(", ", pairs)}}}";
    }
}
