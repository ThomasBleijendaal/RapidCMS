using RapidCMS.Core.Enums;
using RapidCMS.Core.Interfaces.Data;
using RapidCMS.Core.Interfaces.Metadata;

namespace RapidCMS.Core.Models.Data
{
    public class OrderBy : IOrderBy
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
