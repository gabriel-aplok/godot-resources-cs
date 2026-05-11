using GodotResources.Core.Runtime;

namespace GodotResources.Example;

public class ItemData
{
    public string Name { get; set; } = "";
    public int Damage { get; set; }
    public float Weight { get; set; }
    public bool IsTool { get; set; }
    public List<object> Tags { get; set; } = [];
    public ExtResourceReference? Icon { get; set; }
}
