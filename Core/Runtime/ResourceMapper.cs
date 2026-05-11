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

    public static T Map<T>(ResourceSection section)
        where T : class, new()
    {
        // check if a custom converter exists
        if (
            _converters.TryGetValue(typeof(T), out object? converter)
            && converter is IResourceConverter<T> typedConverter
        )
        {
            return typedConverter.Convert(section);
        }

        // fallback to auto reflection-based mapping
        // TODO: fix reflection warnings
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
                if (section.TryGetValue(key, out Variant? variant))
                {
                    try
                    {
                        prop.SetValue(instance, variant.Value);
                    }
                    catch
                    {
                        // fallback to the internal Variant.Get logic for type conversion
                        MethodInfo? method = typeof(Variant)
                            .GetMethod("Get")
                            ?.MakeGenericMethod(prop.PropertyType);
                        object? convertedValue = method?.Invoke(variant, null);
                        prop.SetValue(instance, convertedValue);
                    }
                    break;
                }
            }
        }

        return instance;
    }

    private static string ToSnakeCase(string text)
    {
        return string.Concat(
                text.Select((x, i) => i > 0 && char.IsUpper(x) ? "_" + x : x.ToString())
            )
            .ToLower();
    }
}
