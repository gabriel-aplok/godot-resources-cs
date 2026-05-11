using System.Reflection;

namespace GodotResources.Core.Runtime;

public static class ResourceMapper
{
    private static readonly Dictionary<Type, object> _converters = [];

    public static void RegisterConverter<T>(IResourceConverter<T> converter)
        where T : class, new()
    {
        _converters[typeof(T)] = converter;
    }

    public static T Map<T>(ResourceFile file, ResourceSection section)
        where T : class, new()
    {
        return MapInternal<T>(file, section.Values);
    }

    public static T Map<T>(ResourceFile file, SubResource subResource)
        where T : class, new()
    {
        return MapInternal<T>(file, subResource.Values);
    }

    private static T MapInternal<T>(ResourceFile file, IReadOnlyDictionary<string, Variant> values)
        where T : class, new()
    {
        // check for custom converter
        if (
            _converters.TryGetValue(typeof(T), out object? converter)
            && converter is IResourceConverter<T> typedConverter
        )
        {
            return typedConverter.Convert(values);
        }

        // reflection-based mapping
        T instance = new();
        PropertyInfo[] properties = typeof(T).GetProperties(
            BindingFlags.Public | BindingFlags.Instance
        );

        foreach (PropertyInfo prop in properties)
        {
            if (!prop.CanWrite)
            {
                continue;
            }

            string[] possibleKeys =
            [
                prop.Name,
                char.ToLower(prop.Name[0]) + prop.Name[1..], // camelCase
                ToSnakeCase(prop.Name), // snake_case
            ];

            foreach (string key in possibleKeys)
            {
                if (values.TryGetValue(key, out Variant? variant))
                {
                    prop.SetValue(instance, ResolveValue(file, variant, prop.PropertyType));
                    break;
                }
            }
        }

        return instance;
    }

    private static object? ResolveValue(ResourceFile file, Variant variant, Type targetType)
    {
        object? rawValue = variant.Value;

        if (rawValue is SubResourceReference subRef)
        {
            SubResource? subData = file.GetSubResource(subRef.Id);
            if (subData != null)
            {
                MethodInfo? method = typeof(ResourceMapper)
                    .GetMethod("MapInternal", BindingFlags.NonPublic | BindingFlags.Static)
                    ?.MakeGenericMethod(targetType);
                return method?.Invoke(null, [file, subData.Values]);
            }
        }

        if (rawValue is ExtResourceReference extRef)
        {
            // Here I need ResourceLoader to load the external path
            // then map it. For now, just return the reference or null
            return rawValue;
        }

        return variant.Get(targetType);
    }

    private static string ToSnakeCase(string text)
    {
        return string.Concat(
                text.Select((x, i) => i > 0 && char.IsUpper(x) ? "_" + x : x.ToString())
            )
            .ToLower();
    }
}
