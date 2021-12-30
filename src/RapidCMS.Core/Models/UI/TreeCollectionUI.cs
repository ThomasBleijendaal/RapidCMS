using System;
using RapidCMS.Core.Navigation;

namespace RapidCMS.Core.Models.UI
{
    public class TreeCollectionUI
    {
        public static TreeCollectionUI None = new TreeCollectionUI("empty", "empty", "empty");

        public TreeCollectionUI(string alias, string repositoryAlias, string name)
        {
            Alias = alias ?? throw new ArgumentNullException(nameof(alias));
            Name = name ?? throw new ArgumentNullException(nameof(name));
            RepositoryAlias = repositoryAlias ?? throw new ArgumentNullException(nameof(RepositoryAlias));
        }

        public string Alias { get; private set; }
        public string RepositoryAlias { get; private set; }
        public string Name { get; private set; }
        public string? Icon { get; internal set; }
        public string? Color { get; internal set; }
        public NavigationState? NavigateTo { get; internal set; }
        public bool DefaultOpenEntities { get; internal set; }

        public bool EntitiesVisible { get; internal set; }
        public bool RootVisible { get; internal set; }
    }
}
