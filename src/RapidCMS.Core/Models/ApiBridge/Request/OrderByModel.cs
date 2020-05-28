using System;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Metadata;
using RapidCMS.Core.Enums;

namespace RapidCMS.Core.Models.ApiBridge.Request
{
    public class OrderByModel : IOrderBy
    {
        public OrderByModel(OrderByType type, IPropertyMetadata property)
        {
            OrderByType = type;
            OrderByExpression = property;
        }

        public OrderByType OrderByType { get; private set; }

        public IPropertyMetadata OrderByExpression { get; private set; }

        public IPropertyMetadata? Property => throw new InvalidOperationException("Cannot access this property when using in Api");

        public IExpressionMetadata? Expression => throw new InvalidOperationException("Cannot access this property when using in Api");
    }
}
