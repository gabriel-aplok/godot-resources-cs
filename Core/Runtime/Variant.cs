using System.Collections;

namespace GodotResources.Core.Runtime;

/// <summary>
/// Represents a generic resource value.
/// </summary>
public sealed class Variant(object? value)
{
    public object? Value { get; set; } = value;

    public T Get<T>()
    {
        return (T)Get(typeof(T))!;
    }

    public object? Get(Type targetType)
    {
        if (Value == null)
        {
            return null;
        }

        if (targetType.IsAssignableFrom(Value.GetType()))
        {
            return Value;
        }

        if (Value is IDictionary dict && typeof(IDictionary).IsAssignableFrom(targetType))
        {
            if (
                targetType.IsGenericType
                && targetType.GetGenericTypeDefinition() == typeof(Dictionary<,>)
            )
            {
                Type[] args = targetType.GetGenericArguments();
                IDictionary newDict = (IDictionary)Activator.CreateInstance(targetType)!;

                foreach (DictionaryEntry entry in dict)
                {
                    object? key =
                        (entry.Key is Variant vKey)
                            ? vKey.Get(args[0])
                            : Convert.ChangeType(entry.Key, args[0]);
                    object? val = (entry.Value is Variant vVal) ? vVal.Get(args[1]) : entry.Value;

                    newDict.Add(key!, val);
                }
                return newDict;
            }
        }

        if (Value is IEnumerable enumerable && targetType.IsGenericType)
        {
            Type elementType = targetType.GetGenericArguments()[0];
            Type listType = typeof(List<>).MakeGenericType(elementType);
            IList list = (IList)Activator.CreateInstance(listType)!;

            foreach (object? item in enumerable)
            {
                list.Add(Convert.ChangeType(item, elementType));
            }

            return list;
        }

        if (targetType.IsPrimitive || targetType == typeof(string) || targetType == typeof(decimal))
        {
            return Convert.ChangeType(
                Value,
                targetType,
                System.Globalization.CultureInfo.InvariantCulture
            );
        }

        return Value;
    }

    public override string ToString()
    {
        return Value?.ToString() ?? "null";
    }

    public static implicit operator Variant(string value) => new(value);

    public static implicit operator Variant(int value) => new(value);

    public static implicit operator Variant(float value) => new(value);

    public static implicit operator Variant(bool value) => new(value);
}
