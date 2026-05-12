using GodotResources.Core;
using GodotResources.Core.IO;
using GodotResources.Core.Runtime;
using GodotResources.Core.Utilities;

namespace GodotResources.Example;

public static class Test1
{
    public static void Start()
    {
        ResourceMapper.RegisterConverter(new ItemConverter());

        Logger.Info("Example 1: Mapping with Converter");
        MappingWithConverter();

        Logger.Info("\nExample 2: Manual Access and Modification");
        ManualAccessAndSave();

        Logger.Info("\nExample 3: Complex CRUD and Sub-resources");
        ComplexResourceCreation();
    }

    private static void MappingWithConverter()
    {
        ResourceFile file = ResourceLoader.Load("Data/item.tres");
        ItemData sword = ResourceMapper.Map<ItemData>(file, file.Resource);

        Logger.Info($"Name: {sword.Name}");
        Logger.Info($"Damage: {sword.Damage}");
        Logger.Info($"Is Tool: {sword.IsTool}");
    }

    private static void ManualAccessAndSave()
    {
        ResourceFile file = ResourceLoader.Load("Data/item.tres");

        string name = file.Resource.Get<string>("name");
        int damage = file.Resource.Get<int>("damage");
        List<object> tags = file.Resource.Get<List<object>>("tags");

        Logger.Info($"Name: {name}");
        Logger.Info($"Damage: {damage}");
        Logger.Info($"Tags: {string.Join(", ", tags)}");

        file.Resource["damage"] = new Variant(150);
        file.Resource["tags"] = new Variant(new List<object> { "legendary", "fire" });

        ResourceSaver.Save(file, "Data/item_upgraded.tres");
        Logger.Info("File 'item_upgraded.tres' saved successfully.");
    }

    private static void ComplexResourceCreation()
    {
        ResourceFile file = new()
        {
            ResourceType = "Resource",
            ScriptClass = "ItemData",
            Uid = ResourceUid.Create(),
        };

        file.ExternalResources.Add(
            new ExternalResource
            {
                Id = "1_tex",
                Type = "Texture2D",
                Path = "res://icon.png",
                Uid = ResourceUid.Create(),
            }
        );

        SubResource stats = new("1_stats");
        stats["strength"] = new Variant(10);
        stats["agility"] = new Variant(5);
        file.SubResources.Add(stats);

        ResourceSection main = new("resource");
        main["name"] = new Variant("Espada de Cristal");
        main["damage"] = new Variant(85);

        main["attributes"] = new Variant(new SubResourceReference("1_stats"));
        main["icon"] = new Variant(new ExtResourceReference("1_tex"));

        main["metadata"] = new Variant(
            new Dictionary<string, object>
            {
                ["tier"] = "rare",
                ["effects"] = new List<object> { "freeze", "slow" },
            }
        );

        file.Sections.Add(main);

        ResourceSaver.Save(file, "Data/complex_item.tres");

        ResourceFile loaded = ResourceLoader.Load("Data/complex_item.tres");
        Logger.Info($"Complex resource saved and reloaded: {loaded.Resource.Get<string>("name")}");

        Dictionary<string, object> metadata = loaded.Resource.Get<Dictionary<string, object>>(
            "metadata"
        );
        Logger.Info($"Tier: {metadata["tier"]}");
    }
}
