using System.Collections.Generic;
using RapidCMS.Common.Models.UI;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Extensions;
using RapidCMS.Core.Models.Data;

namespace RapidCMS.Core.Models.UI
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
        // null ref?
        public IEnumerable<IOrderBy>? OrderBys => CommonFields?.SelectNotNull(x => new OrderBy(x.SortDescending, x.OrderByExpression));
    }
}
