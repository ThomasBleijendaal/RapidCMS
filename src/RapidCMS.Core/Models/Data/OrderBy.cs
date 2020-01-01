using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Metadata;
using RapidCMS.Core.Enums;

namespace RapidCMS.Core.Models.Data
{
    internal class OrderBy : IOrderBy
    {
        public OrderBy(OrderByType orderByType, IPropertyMetadata orderByExpression)
        {
            OrderByType = orderByType;
            OrderByExpression = orderByExpression;
        }

        public OrderByType OrderByType { get; private set; }

        public IPropertyMetadata OrderByExpression { get; private set; }
    }
}
