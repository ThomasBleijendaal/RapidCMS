using System;
using System.Collections.Generic;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Navigation;

namespace RapidCMS.Core.Models.UI;

public class TreeNodeUI
{
    public TreeNodeUI(string id, string repositoryAlias, string name, List<(string alias, PageType type)> collections)
    {
        Id = id ?? throw new ArgumentNullException(nameof(id));
        RepositoryAlias = repositoryAlias ?? throw new ArgumentNullException(nameof(repositoryAlias));
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Collections = collections ?? throw new ArgumentNullException(nameof(collections));
    }

    public string Id { get; private set; }
    public string RepositoryAlias { get; private set; }
    public string Name { get; private set; }
    public NavigationState? NavigateTo { get; internal set; }
    public bool RootVisibleOfCollections { get; internal set; }
    public bool DefaultOpenCollections { get; internal set; }
    public List<(string alias, PageType type)> Collections { get; private set; }
}
