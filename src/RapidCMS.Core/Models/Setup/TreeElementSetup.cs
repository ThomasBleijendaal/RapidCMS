using System;
using RapidCMS.Core.Enums;

namespace RapidCMS.Core.Models.Setup;

public class TreeElementSetup
{
    public TreeElementSetup(string alias, string name, PageType type)
    {
        Alias = alias ?? throw new ArgumentNullException(nameof(alias));
        Name = name ?? throw new ArgumentNullException(nameof(alias));
        Type = type;
    }

    public string Alias { get; }

    public string Name { get; }

    public PageType Type { get; }

    public CollectionRootVisibility RootVisibility { get; internal set; }
}
