using GodotResources.Core.Runtime;

namespace GodotResources.Example;

public class ItemConverter : IResourceConverter<ItemData>
{
    public ItemData Convert(IReadOnlyDictionary<string, Variant> values)
    {
        return new ItemData
        {
            Name = values["name"].Get<string>().ToUpper(),
            Damage = values["damage"].Get<int>() + 10,
            IsTool = values["is_tool"].Get<bool>(),
            Tags = values["tags"].Get<List<object>>(),
        };
    }
}
