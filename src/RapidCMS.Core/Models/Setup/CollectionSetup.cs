using System;
using System.Collections.Generic;
using System.Linq;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Setup;
using RapidCMS.Core.Enums;
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
            string repositoryAlias)
        {
            Icon = icon;
            Color = color;
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Alias = alias ?? throw new ArgumentNullException(nameof(alias));
            RepositoryAlias = repositoryAlias ?? throw new ArgumentNullException(nameof(repositoryAlias));
            Validators = new List<Type>();
        }

        public string? Icon { get; private set; }
        public string? Color { get; private set; }
        public string Name { get; private set; }
        public string Alias { get; private set; }
        public string RepositoryAlias { get; private set; }

        public UsageType UsageType { get; set; }

        public ITreeElementSetup? Parent { get; set; }
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

        public ITreeViewSetup? TreeView { get; set; }
        public IElementSetup? ElementSetup { get; set; }

        public IListSetup? ListView { get; set; }
        public IListSetup? ListEditor { get; set; }

        public INodeSetup? NodeView { get; set; }
        public INodeSetup? NodeEditor { get; set; }

        public List<Type> Validators { get; set; }

        public IButtonSetup? FindButton(string buttonId)
            => EnumerableExtensions
                .MergeAll(
                    ListView?.GetAllButtons(),
                    ListEditor?.GetAllButtons(),
                    NodeView?.GetAllButtons(),
                    NodeEditor?.GetAllButtons())
                .FirstOrDefault(x => x.ButtonId == buttonId);
    }
}
