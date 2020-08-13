using System;
using System.Linq;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Helpers;

namespace RapidCMS.Core.Models.ApiBridge.Request
{
    public class ParentQueryModel : QueryModel
    {
        public ParentQueryModel() { }

        public ParentQueryModel(IParent? parent)
        {
            ParentPath = parent?.GetParentPath()?.ToPathString();
        }

        public ParentQueryModel(IParent? parent, IQuery query) : this(parent)
        {
            Skip = query.Skip;
            Take = query.Take;
            SearchTerm = query.SearchTerm;
            ActiveTab = query.ActiveTab;

            CollectionAlias = query.CollectionAlias ?? throw new ArgumentNullException(nameof(CollectionAlias));

            OrderBys = query.ActiveOrderBys
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

        public ParentQueryModel(IParent? parent, Type? variantType, IQuery query) : this(parent, query)
        {
            VariantAlias = variantType == null ? null : AliasHelper.GetEntityVariantAlias(variantType);
        }

        public string? ParentPath { get; set; }
    }
}
