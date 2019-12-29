using RapidCMS.Common.Enums;
using RapidCMS.Common.Models.Metadata;

namespace RapidCMS.Common.Data
{
    public class OrderBy : IOrderBy
    {
        public OrderBy(OrderByType orderByType, IPropertyMetadata? orderByExpression)
        {
            OrderByType = orderByType;
            OrderByExpression = orderByExpression;
        }

        public OrderByType OrderByType { get; private set; }

        public IPropertyMetadata? OrderByExpression { get; private set; }
    }
}
