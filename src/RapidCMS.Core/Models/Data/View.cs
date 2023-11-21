using System.Collections.Generic;
using System.Linq;
using RapidCMS.Core.Abstractions.Data;

namespace RapidCMS.Core.Models.Data;

public sealed class View : IView
{
    internal IDataView? DataView;
    internal IEnumerable<IOrderBy>? OrderBys;

    public static View Default(string? collectionAlias = default)
    {
        return new View
        {
            Skip = 0,
            Take = 1000,
            CollectionAlias = collectionAlias
        };
    }

    public static View Create(int pageSize, int pageNumber, string? searchTerm, int? activeTab, string? collectionAlias = default)
    {
        return new View
        {
            Skip = pageSize * (pageNumber - 1),
            Take = pageSize,
            SearchTerm = searchTerm,
            ActiveTab = activeTab,
            CollectionAlias = collectionAlias
        };
    }

    public static View Create(IView view, string collectionAlias)
    {
        return new View
        {
            Skip = view.Skip,
            Take = view.Take,
            SearchTerm = view.SearchTerm,
            ActiveTab = view.ActiveTab,
            CollectionAlias = collectionAlias
        };
    }

    public void HasMoreData(bool hasMoreData)
    {
        MoreDataAvailable = hasMoreData;
    }

    public void SetDataView(IDataView dataView)
    {
        DataView = dataView;
    }

    public void SetOrderBys(IEnumerable<IOrderBy>? orderBys)
    {
        OrderBys = orderBys;
    }

    public int Skip { get; private set; }
    public int Take { get; private set; }

    public string? SearchTerm { get; private set; }

    public int? ActiveTab { get; private set; }

    public bool MoreDataAvailable { get; private set; } = false;

    public IDataView? ActiveDataView => DataView;

    public IEnumerable<IOrderBy> ActiveOrderBys => OrderBys ?? Enumerable.Empty<IOrderBy>();

    public string? CollectionAlias { get; set; }
}
