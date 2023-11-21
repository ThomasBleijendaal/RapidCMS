using System;

namespace RapidCMS.Core.Models.Setup;

public class EntityVariantSetup 
{
    public static EntityVariantSetup Undefined = new EntityVariantSetup();

    private EntityVariantSetup()
    {
        Name = "";
        Type = typeof(object);
        Alias = "";
    }

    public EntityVariantSetup(string name, string? icon, Type type, string alias)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Icon = icon;
        Type = type ?? throw new ArgumentNullException(nameof(type));
        Alias = alias ?? throw new ArgumentNullException(nameof(alias));
    }

    public string Name { get; internal set; }
    public string? Icon { get; internal set; }
    public Type Type { get; internal set; }
    public string Alias { get; internal set; }
}
