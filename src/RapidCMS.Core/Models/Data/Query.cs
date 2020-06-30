using System.Collections.Generic;
using System.Linq;
using RapidCMS.Core.Abstractions.Data;

namespace RapidCMS.Core.Models.Data
{
    public sealed class Query : IQuery
    {
        internal IDataView? DataView;
        internal IEnumerable<IOrderBy>? OrderBys;

        public static Query Default()
        {
            return new Query
            {
                Skip = 0,
                Take = 1000
            };
        }

        public static Query Create(int pageSize, int pageNumber, string? searchTerm, int? activeTab)
        {
            return new Query
            {
                Skip = pageSize * (pageNumber - 1),
                Take = pageSize,
                SearchTerm = searchTerm,
                ActiveTab = activeTab
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
    }
}
