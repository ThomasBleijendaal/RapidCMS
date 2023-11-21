using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Extensions;
using RapidCMS.Core.Models.Setup;

namespace RapidCMS.Core.Resolvers.Data;

internal class FormDataViewResolver : IDataViewResolver
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ISetupResolver<CollectionSetup> _collectionResolver;

    public FormDataViewResolver(
        IServiceProvider serviceProvider,
        ISetupResolver<CollectionSetup> collectionResolver)
    {
        _serviceProvider = serviceProvider;
        _collectionResolver = collectionResolver;
    }

    public async Task ApplyDataViewToViewAsync(IView view)
    {
        if (string.IsNullOrEmpty(view.CollectionAlias))
        {
            throw new ArgumentNullException($"{nameof(view)}.{nameof(view.CollectionAlias)}");
        }

        var collection = await _collectionResolver.ResolveSetupAsync(view.CollectionAlias);

        if (collection.DataViewBuilder != null || collection.DataViews?.Count > 0)
        {
            var dataViews = await GetDataViewsAsync(collection);
            var dataView = dataViews.FirstOrDefault(x => x.Id == view.ActiveTab)
                ?? dataViews.FirstOrDefault();

            if (dataView != null)
            {
                view.SetDataView(dataView);
            }
        }
    }

    public async Task<IEnumerable<IDataView>> GetDataViewsAsync(string collectionAlias)
    {
        var collection = await _collectionResolver.ResolveSetupAsync(collectionAlias);
        return await GetDataViewsAsync(collection);
    }

    private Task<IEnumerable<IDataView>> GetDataViewsAsync(CollectionSetup collection)
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
