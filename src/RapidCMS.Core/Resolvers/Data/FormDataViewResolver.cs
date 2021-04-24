using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Abstractions.Setup;
using RapidCMS.Core.Extensions;

namespace RapidCMS.Core.Resolvers.Data
{
    internal class FormDataViewResolver : IDataViewResolver
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ISetupResolver<ICollectionSetup> _collectionResolver;

        public FormDataViewResolver(
            IServiceProvider serviceProvider,
            ISetupResolver<ICollectionSetup> collectionResolver)
        {
            _serviceProvider = serviceProvider;
            _collectionResolver = collectionResolver;
        }

        public async Task ApplyDataViewToQueryAsync(IQuery query)
        {
            if (string.IsNullOrEmpty(query.CollectionAlias))
            {
                throw new ArgumentNullException($"{nameof(query)}.{nameof(query.CollectionAlias)}");
            }

            var collection = await _collectionResolver.ResolveSetupAsync(query.CollectionAlias);

            if (collection.DataViewBuilder != null || collection.DataViews?.Count > 0)
            {
                var dataViews = await GetDataViewsAsync(collection);
                var dataView = dataViews.FirstOrDefault(x => x.Id == query.ActiveTab)
                    ?? dataViews.FirstOrDefault();

                if (dataView != null)
                {
                    query.SetDataView(dataView);
                }
            }
        }

        public async Task<IEnumerable<IDataView>> GetDataViewsAsync(string collectionAlias)
        {
            var collection = await _collectionResolver.ResolveSetupAsync(collectionAlias);
            return await GetDataViewsAsync(collection);
        }

        private Task<IEnumerable<IDataView>> GetDataViewsAsync(ICollectionSetup collection)
        {
            if (collection.DataViewBuilder == null)
            {
                return Task.FromResult(collection.DataViews ?? Enumerable.Empty<IDataView>());
            }
            else
            {
                var builder = _serviceProvider.GetService<IDataViewBuilder>(collection.DataViewBuilder);
                return builder.GetDataViewsAsync();
            }
        }
    }
}
