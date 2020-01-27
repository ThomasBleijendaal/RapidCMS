using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Metadata;
using RapidCMS.Core.Enums;

namespace RapidCMS.Core.Models.Data
{
    internal class OrderBy : IOrderBy
    {
        public OrderBy(OrderByType orderByType, IPropertyMetadata orderByExpression, IPropertyMetadata? property, IExpressionMetadata? expression)
        {
            OrderByType = orderByType;
            OrderByExpression = orderByExpression;
            Expression = expression;
            Property = property;
        }

        public OrderByType OrderByType { get; private set; }

        public IPropertyMetadata OrderByExpression { get; private set; }

        public IPropertyMetadata? Property { get; private set; }
        public IExpressionMetadata? Expression { get; private set; }
    }
}
