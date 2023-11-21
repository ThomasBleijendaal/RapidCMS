using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using RapidCMS.Core.Abstractions.Config;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Extensions;

namespace RapidCMS.Core.Resolvers.Data;

internal class ApiDataViewResolver : IDataViewResolver
{
    private readonly IReadOnlyDictionary<string, Type> _dataViewTypes;
    private readonly IServiceProvider _serviceProvider;

    public ApiDataViewResolver(
        IApiConfig apiConfig,
        IServiceProvider serviceProvider)
    {
        _dataViewTypes = apiConfig.DataViews.ToDictionary(dataView => dataView.Alias, dataView => dataView.DataViewBuilder);
        _serviceProvider = serviceProvider;
    }

    public async Task ApplyDataViewToViewAsync(IView view)
    {
        if (string.IsNullOrEmpty(view.CollectionAlias))
        {
            throw new ArgumentNullException($"{nameof(view)}.{nameof(view.CollectionAlias)}");
        }
            
        var dataViews = await GetDataViewsAsync(view.CollectionAlias);
        var dataView = dataViews.FirstOrDefault(x => x.Id == view.ActiveTab)
                ?? dataViews.FirstOrDefault();

        if (dataView != null)
        {
            view.SetDataView(dataView);
        }
    }

    public Task<IEnumerable<IDataView>> GetDataViewsAsync(string collectionAlias)
    {
        if (!_dataViewTypes.TryGetValue(collectionAlias, out var dataView))
        {
            return Task.FromResult(Enumerable.Empty<IDataView>());
        }

        var builder = _serviceProvider.GetService<IDataViewBuilder>(dataView);
        return builder.GetDataViewsAsync();
    }
}
