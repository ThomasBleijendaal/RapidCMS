﻿using System.Collections.Generic;
using System.Linq;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Models.Data;
using RapidCMS.Core.Navigation;

namespace RapidCMS.Core.Models.UI;

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
    public bool Reorderable { get; internal set; }

    public IEnumerable<IOrderBy>? GetOrderBys(SortBag? userSorts, SortBag? tabSorts) 
        => CommonFields?
            .Where(x => x.OrderByExpression != null)
            .Select(x => new OrderBy(userSorts?.Get(x.Index) ?? tabSorts?.Get(x.Index) ?? x.SortDirection, x.OrderByExpression!, x.Property, x.Expression));
}
