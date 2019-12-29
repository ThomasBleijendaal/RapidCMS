using System.Collections.Generic;
using RapidCMS.Common.Data;
using RapidCMS.Common.Enums;
using RapidCMS.Common.Extensions;

namespace RapidCMS.Common.Models.UI
{
    public class ListUI
    {
        public ListType ListType { get; internal set; }
        public EmptyVariantColumnVisibility EmptyVariantColumnVisibility { get; internal set; }

        public List<FieldUI>? UniqueFields { get; internal set; }
        public List<FieldUI>? CommonFields { get; internal set; }
        public int MaxUniqueFieldsInSingleEntity { get; internal set; }
        public bool SectionsHaveButtons { get; internal set; }

        public int PageSize { get; internal set; }
        public bool SearchBarVisible { get; internal set; }

        // TODO: join with unique fields?
        public IEnumerable<IOrderBy>? OrderBys => CommonFields?.SelectNotNull(x => new OrderBy(x.SortDescending, x.OrderByExpression));
    }
}
