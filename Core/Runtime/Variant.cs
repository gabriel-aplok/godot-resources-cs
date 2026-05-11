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
        if (Value is T value)
        {
            return value;
        }

        if (Value == null)
        {
            return default!;
        }

        if (Value is IEnumerable enumerable && typeof(T).IsGenericType)
        {
            Type targetType = typeof(T).GetGenericArguments()[0];
            Type listType = typeof(List<>).MakeGenericType(targetType);
            IList list = (IList)Activator.CreateInstance(listType)!;

            foreach (object? item in enumerable)
            {
                list.Add(Convert.ChangeType(item, targetType));
            }

            return (T)list;
        }

        return (T)
            Convert.ChangeType(Value, typeof(T), System.Globalization.CultureInfo.InvariantCulture);
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
