using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RapidCMS.Common.Data;
using RapidCMS.Common.Extensions;

namespace RapidCMS.Common.Models
{
    public class Collection
    {
        public Collection(string name, string alias, EntityVariant entityVariant, Type repositoryType, bool isRecursive = false)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Alias = alias ?? throw new ArgumentNullException(nameof(alias));
            EntityVariant = entityVariant ?? throw new ArgumentNullException(nameof(entityVariant));
            RepositoryType = repositoryType ?? throw new ArgumentNullException(nameof(repositoryType));

            Recursive = isRecursive;
        }

        internal string Name { get; private set; }
        internal string Alias { get; private set; }
        internal bool Recursive { get; private set; }

        public List<Collection> Collections { get; set; } = new List<Collection>();

        internal List<EntityVariant>? SubEntityVariants { get; set; }
        internal EntityVariant EntityVariant { get; private set; }

        internal List<IDataView> DataViews { get; set; }
        internal Type? DataViewBuilder { get; set; }

        internal EntityVariant GetEntityVariant(string? alias)
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
        internal EntityVariant GetEntityVariant(IEntity entity)
        {
            return SubEntityVariants?.FirstOrDefault(x => x.Type == entity.GetType())
                ?? EntityVariant;
        }

        internal Task<IEnumerable<IDataView>> GetDataViewsAsync(IServiceProvider serviceProvider)
        {
            if (DataViewBuilder == null)
            {
                return Task.FromResult((IEnumerable<IDataView>)DataViews);
            }
            else
            {
                var builder = serviceProvider.GetService<IDataViewBuilder>(DataViewBuilder);
                return builder.GetDataViewsAsync();
            }
        }

        internal async Task ProcessDataViewAsync(Query query, IServiceProvider serviceProvider)
        {
            if (DataViewBuilder != null || DataViews.Count > 0)
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

        internal Type RepositoryType { get; private set; }
        internal IRepository Repository { get; set; }

        internal TreeView? TreeView { get; set; }

        internal List? ListView { get; set; }
        internal List? ListEditor { get; set; }

        internal Node? NodeView { get; set; }
        internal Node? NodeEditor { get; set; }
    }
}
