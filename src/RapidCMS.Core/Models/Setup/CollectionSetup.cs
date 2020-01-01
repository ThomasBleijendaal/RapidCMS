using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Repositories;
using RapidCMS.Core.Extensions;
using RapidCMS.Core.Models.Data;

namespace RapidCMS.Core.Models.Setup
{
    internal class CollectionSetup
    {
        internal CollectionSetup(
            string? icon, 
            string name, 
            string alias, 
            EntityVariantSetup entityVariant, 
            Type? repositoryType,
            bool isRecursive = false)
        {
            Icon = icon;
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Alias = alias ?? throw new ArgumentNullException(nameof(alias));
            EntityVariant = entityVariant ?? throw new ArgumentNullException(nameof(entityVariant));
            RepositoryType = repositoryType;

            Recursive = isRecursive;
        }

        internal string? Icon { get; private set; }
        internal string Name { get; private set; }
        internal string Alias { get; private set; }
        internal bool Recursive { get; private set; }

        public List<CollectionSetup> Collections { get; set; } = new List<CollectionSetup>();

        internal List<EntityVariantSetup>? SubEntityVariants { get; set; }
        internal EntityVariantSetup EntityVariant { get; private set; }

        internal List<IDataView>? DataViews { get; set; }
        internal Type? DataViewBuilder { get; set; }

        internal EntityVariantSetup GetEntityVariant(string? alias)
        {
            if (string.IsNullOrWhiteSpace(alias) || SubEntityVariants == null)
            {
                return EntityVariant;
            }
            else
            {
                return SubEntityVariants.First(x => x.Alias == alias);
            }
        }
        internal EntityVariantSetup GetEntityVariant(IEntity entity)
        {
            return SubEntityVariants?.FirstOrDefault(x => x.Type == entity.GetType())
                ?? EntityVariant;
        }

        internal Task<IEnumerable<IDataView>> GetDataViewsAsync(IServiceProvider serviceProvider)
        {
            if (DataViewBuilder == null)
            {
                return Task.FromResult(DataViews ?? Enumerable.Empty<IDataView>());
            }
            else
            {
                var builder = serviceProvider.GetService<IDataViewBuilder>(DataViewBuilder);
                return builder.GetDataViewsAsync();
            }
        }

        internal async Task ProcessDataViewAsync(Query query, IServiceProvider serviceProvider)
        {
            if (DataViewBuilder != null || DataViews?.Count > 0)
            {
                var dataViews = await GetDataViewsAsync(serviceProvider);

                var dataView = dataViews.FirstOrDefault(x => x.Id == query.ActiveTab)
                    ?? dataViews.FirstOrDefault();

                if (dataView != null)
                {
                    query.SetDataViewExpression(dataView.QueryExpression);
                }
            }
        }

        internal Type? RepositoryType { get; private set; }
        
        internal TreeViewSetup? TreeView { get; set; }

        internal ListSetup? ListView { get; set; }
        internal ListSetup? ListEditor { get; set; }

        internal NodeSetup? NodeView { get; set; }
        internal NodeSetup? NodeEditor { get; set; }

        internal ButtonSetup? FindButton(string buttonId)
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
