using System;
using System.Collections.Generic;
using System.Linq;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Helpers;
using static RapidCMS.Core.Models.Data.View;

namespace RapidCMS.Core.Models.ApiBridge.Request;

public class QueryModel
{
    public int Skip { get; set; }
    public int Take { get; set; }
    public string? SearchTerm { get; set; }

    public int? ActiveTab { get; set; }

    public string? VariantAlias { get; set; }

    public string? CollectionAlias { get; set; }

    public IEnumerable<OrderModel>? OrderBys { get; set; }

    public IView GetView<TEntity>() => GetView(typeof(TEntity));

    public IView GetView(Type entityType)
    {
        var view = Create(Take, 1 + Skip / Math.Max(1, Take), SearchTerm, ActiveTab);

        view.CollectionAlias = CollectionAlias;

        if (OrderBys != null)
        {
            view.SetOrderBys(OrderBys
                .Select(x =>
                {
                    var property = PropertyMetadataHelper.GetPropertyMetadata(entityType, x.PropertyName);

                    if (property == null || x.Fingerprint != property.Fingerprint)
                    {
                        throw new InvalidOperationException("The used orderByExpression could not be converted back into IPropertyMetadata. " +
                            "Only properties are supported for conversion, so x.Property or x.Property.NestedProperty. More complicated expressions cannot " +
                            "be converted to and from strings easily and will not work with ApiRepositories. If these complex expressions are required, " +
                            "please convert to ServerSide RapidCMS.");
                    }

                    return new OrderByModel(x.OrderByType, property);
                }));
        }
        return view;
    }
}
