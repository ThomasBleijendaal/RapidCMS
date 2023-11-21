using System;
using System.Linq;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Helpers;

namespace RapidCMS.Core.Models.ApiBridge.Request;

public class ParentQueryModel : QueryModel
{
    public ParentQueryModel() { }

    public ParentQueryModel(IParent? parent)
    {
        ParentPath = parent?.GetParentPath()?.ToPathString();
    }

    public ParentQueryModel(IParent? parent, IView view) : this(parent)
    {
        Skip = view.Skip;
        Take = view.Take;
        SearchTerm = view.SearchTerm;
        ActiveTab = view.ActiveTab;

        CollectionAlias = view.CollectionAlias ?? throw new ArgumentNullException(nameof(CollectionAlias));

        OrderBys = view.ActiveOrderBys
            .Select(x => new OrderModel
            {
                PropertyName = x.OrderByExpression.PropertyName,
                Fingerprint = x.OrderByExpression.Fingerprint,
                OrderByType = x.OrderByType
            });
    }

    public ParentQueryModel(IParent? parent, Type? variantType) : this(parent)
    {
        VariantAlias = variantType == null ? null : AliasHelper.GetEntityVariantAlias(variantType);
    }

    public ParentQueryModel(IParent? parent, Type? variantType, IView query) : this(parent, query)
    {
        VariantAlias = variantType == null ? null : AliasHelper.GetEntityVariantAlias(variantType);
    }

    public string? ParentPath { get; set; }
}
