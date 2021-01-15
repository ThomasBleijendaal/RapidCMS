using System;
using System.Collections.Generic;
using System.Linq;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Setup;
using RapidCMS.Core.Exceptions;
using RapidCMS.Core.Extensions;

namespace RapidCMS.Core.Models.Setup
{
    internal class CollectionSetup : ICollectionSetup
    {
        internal CollectionSetup(
            string? icon,
            string? color,
            string name,
            string alias,
            string repositoryAlias,
            bool isRecursive = false,
            bool isResolverCachable = true) // TODO
        {
            Icon = icon;
            Color = color;
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Alias = alias ?? throw new ArgumentNullException(nameof(alias));
            RepositoryAlias = repositoryAlias ?? throw new ArgumentNullException(nameof(repositoryAlias));

            Recursive = isRecursive;
        }

        public string? Icon { get; private set; }
        public string? Color { get; private set; }
        public string Name { get; private set; }
        public string Alias { get; private set; }
        public string RepositoryAlias { get; private set; }
        public bool Recursive { get; private set; }

        public List<ITreeElementSetup> Collections { get; set; } = new List<ITreeElementSetup>();

        public List<IEntityVariantSetup>? SubEntityVariants { get; set; }
        public IEntityVariantSetup EntityVariant { get; set; } = EntityVariantSetup.Undefined;

        public List<IDataView>? DataViews { get; set; }
        public Type? DataViewBuilder { get; set; }

        public IEntityVariantSetup GetEntityVariant(string? alias)
        {
            if (string.IsNullOrWhiteSpace(alias) || SubEntityVariants == null || EntityVariant.Alias == alias)
            {
                return EntityVariant;
            }
            else
            {
                return SubEntityVariants.FirstOrDefault(x => x.Alias == alias) ?? throw new InvalidOperationException($"Entity variant with alias {alias} does not exist.");
            }
        }
        public IEntityVariantSetup GetEntityVariant(IEntity entity)
        {
            return SubEntityVariants?.FirstOrDefault(x => x.Type == entity.GetType())
                ?? EntityVariant;
        }

        public TreeViewSetup? TreeView { get; set; }

        public ListSetup? ListView { get; set; }
        public ListSetup? ListEditor { get; set; }

        public NodeSetup? NodeView { get; set; }
        public NodeSetup? NodeEditor { get; set; }

        public IButtonSetup? FindButton(string buttonId)
        {
            return EnumerableExtensions
                .MergeAll(
                    ListView?.GetAllButtons(),
                    ListEditor?.GetAllButtons(),
                    NodeView?.GetAllButtons(),
                    NodeEditor?.GetAllButtons())
                .FirstOrDefault(x => x.ButtonId == buttonId);
        }
    }
}
