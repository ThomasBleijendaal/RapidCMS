using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Abstractions.Setup;
using RapidCMS.Core.Extensions;
using RapidCMS.Core.Models.Data;

namespace RapidCMS.Core.Resolvers.Data
{
    internal class FormDataViewResolver : IDataViewResolver
    {
        private readonly IServiceProvider _serviceProvider;

        public FormDataViewResolver(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task ApplyDataViewToQueryAsync(IQuery query, ICollectionSetup collection)
        {
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

        public Task<IEnumerable<IDataView>> GetDataViewsAsync(ICollectionSetup collection)
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
