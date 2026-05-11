using GodotResources.Core.Runtime;

namespace GodotResources.Example;

public class ItemConverter : IResourceConverter<ItemData>
{
    public ItemData Convert(ResourceSection section)
    {
        return new ItemData
        {
            Name = section.Get<string>("name").ToUpper(),
            Damage = section.Get<int>("damage") + 10,
            IsTool = section.Get<bool>("is_tool"),
            Tags = section.Get<List<object>>("tags"),
        };
    }
}
