using System.Collections.Generic;
using System.Linq;
using RapidCMS.Core.Abstractions.Data;

namespace RapidCMS.Core.Models.Data
{
    public sealed class Query : IQuery
    {
        internal IDataView? DataView;
        internal IEnumerable<IOrderBy>? OrderBys;

        public static Query Default(string? collectionAlias = default)
        {
            return new Query
            {
                Skip = 0,
                Take = 1000,
                CollectionAlias = collectionAlias
            };
        }

        public static Query Create(int pageSize, int pageNumber, string? searchTerm, int? activeTab, string? collectionAlias = default)
        {
            return new Query
            {
                Skip = pageSize * (pageNumber - 1),
                Take = pageSize,
                SearchTerm = searchTerm,
                ActiveTab = activeTab,
                CollectionAlias = collectionAlias
            };
        }

        public static Query Create(IQuery query, string collectionAlias)
        {
            return new Query
            {
                Skip = query.Skip,
                Take = query.Take,
                SearchTerm = query.SearchTerm,
                ActiveTab = query.ActiveTab,
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

        public void SetOrderBys(IEnumerable<IOrderBy> orderBys)
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
}
