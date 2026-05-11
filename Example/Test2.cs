using GodotResources.Core.IO;
using GodotResources.Core.Runtime;
using GodotResources.Core.Utilities;

namespace GodotResources.Example;

public static class Test2
{
    public static void Start()
    {
        Console.WriteLine("==== Example 1 ====");
        ResourceMapper.RegisterConverter(new ItemConverter());
        ResourceFile file = ResourceLoader.Load("Data/item.tres");
        ItemData sword = ResourceMapper.Map<ItemData>(file, file.Resource);
        Console.WriteLine($"Name: {sword.Name}");
        Console.WriteLine($"Damage: {sword.Damage}");
        Console.WriteLine($"IsTool: {sword.IsTool}");

        Console.WriteLine("==== Example 2 ====");
        SimpleResLoad();

        Console.WriteLine("==== Example 3 ====");
        ComplexCRUD();
    }

    private static void SimpleResLoad()
    {
        ResourceFile file = ResourceLoader.Load("Data/item.tres");

        string name = file.Resource.Get<string>("name");
        int damage = file.Resource.Get<int>("damage");
        float weight = file.Resource.Get<float>("weight");
        bool is_tool = file.Resource.Get<bool>("is_tool");
        List<object> tags = file.Resource.Get<List<object>>("tags");

        Console.WriteLine($"Loaded Item:");
        Console.WriteLine($"Name: {name}");
        Console.WriteLine($"Damage: {damage}");
        Console.WriteLine($"Weight: {weight}");
        Console.WriteLine($"IsTool: {is_tool}");
        Console.WriteLine($"Tags: {string.Join(", ", tags)}");

        // Modify values
        file.Resource["damage"] = new Variant(25);
        file.Resource["weight"] = new Variant(2.0f);
        file.Resource["is_tool"] = new Variant(false);
        file.Resource["description"] = new Variant("Modified item");

        // Save
        ResourceSaver.Save(file, "Data/item_modified.tres");

        Console.WriteLine("Saved modified resource.");
    }

    private static void ComplexCRUD()
    {
        // create a new resource file
        ResourceFile file = new()
        {
            ResourceType = "Resource",
            ScriptClass = "ItemData",
            FormatVersion = 3,
            Uid = ResourceUid.Create(),
        };
        // add external resources
        file.ExternalResources.Add(
            new ExternalResource
            {
                Id = "1_texture",
                Type = "Texture2D",
                Path = "res://textures/sword.png",
                Uid = ResourceUid.Create(),
            }
        );
        file.ExternalResources.Add(
            new ExternalResource
            {
                Id = "1_audio",
                Type = "AudioStream",
                Path = "res://audio/swing.wav",
                Uid = ResourceUid.Create(),
            }
        );

        // create main resource section
        ResourceSection resource = new("resource");

        // strings
        resource["name"] = new Variant("iron sword");
        resource["description"] = new Variant("a powerful melee weapon");
        // integers
        resource["damage"] = new Variant(25);
        resource["durability"] = new Variant(120);
        // floats
        resource["weight"] = new Variant(2.5f);
        resource["crit_chance"] = new Variant(0.15f);
        // booleans
        resource["is_tool"] = new Variant(true);
        resource["two_handed"] = new Variant(false);
        // null
        resource["owner"] = new Variant(null);

        // arrays
        resource["tags"] = new Variant(new List<object> { "weapon", "melee", "rare", "starter" });
        // nested arrays
        resource["spawn_points"] = new Variant(
            new List<object>
            {
                new List<object> { 10, 20 },
                new List<object> { 30, 40 },
                new List<object> { 50, 60 },
            }
        );
        // dictionaries
        resource["stats"] = new Variant(
            new Dictionary<string, object>
            {
                ["damage"] = 25,
                ["crit"] = 0.15f,
                ["attack_speed"] = 1.2f,
                ["knockback"] = true,
            }
        );
        // nested dictionary
        resource["metadata"] = new Variant(
            new Dictionary<string, object>
            {
                ["author"] = "gabriel",
                ["version"] = 1,
                ["properties"] = new Dictionary<string, object>
                {
                    ["can_sell"] = true,
                    ["max_stack"] = 1,
                },
            }
        );

        // external resource reference
        resource["icon"] = new Variant(new ExtResourceReference("1_texture"));
        // another external resource reference
        resource["swing_audio"] = new Variant(new ExtResourceReference("1_audio"));
        // subresource reference
        resource["material"] = new Variant(new SubResourceReference("1_material"));
        // slash-separated metadata keys
        resource["metadata/custom_value"] = new Variant("hello world");
        resource["metadata/category"] = new Variant("equipment");

        // multiline text
        resource["lore"] = new Variant(
            """
            forged long ago by ancient blacksmiths.
            this sword is said to contain magical powers.
            """
        );

        // add section to file
        file.Sections.Add(resource);

        // create a subresource section
        ResourceSection material = new("sub_resource");

        material["id"] = new Variant("1_material");
        material["shader"] = new Variant("res://shaders/sword.shader");
        material["metallic"] = new Variant(0.85f);
        material["roughness"] = new Variant(0.2f);

        file.Sections.Add(material);

        // save file
        ResourceSaver.Save(file, "Data/complete_item.tres");
        Console.WriteLine("resource saved successfully.");

        // load it again
        ResourceFile loaded = ResourceLoader.Load("Data/complete_item.tres");

        // read values
        string itemName = loaded.Resource.Get<string>("name");
        int damage = loaded.Resource.Get<int>("damage");
        float crit = loaded.Resource.Get<float>("crit_chance");
        bool is_tool = loaded.Resource.Get<bool>("is_tool");

        Console.WriteLine($"name: {itemName}");
        Console.WriteLine($"damage: {damage}");
        Console.WriteLine($"crit: {crit}");
        Console.WriteLine($"is_tool: {is_tool}");

        // read array values
        List<object> tags = loaded.Resource.Get<List<object>>("tags");
        Console.WriteLine($"tags: {string.Join(", ", tags)}");

        // read nested array values
        List<object> spawnPoints = loaded.Resource.Get<List<object>>("spawn_points");
        Console.WriteLine($"spawn_points: {string.Join(", ", spawnPoints)}");

        // read multiline text
        string lore = loaded.Resource.Get<string>("lore");
        Console.WriteLine($"lore: {lore}");

        // modify values after loading
        loaded.Resource["tags"] = new Variant(new List<object> { "a", "b", "c", "d" });
        loaded.Resource["damage"] = new Variant(999);
        loaded.Resource["description"] = new Variant("my little pony");

        // save modified file
        ResourceSaver.Save(loaded, "Data/complete_item_modified.tres");
        Console.WriteLine("modified resource saved.");
    }
}
