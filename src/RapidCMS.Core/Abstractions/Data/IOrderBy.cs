using RapidCMS.Core.Abstractions.Metadata;
using RapidCMS.Core.Enums;

namespace RapidCMS.Core.Abstractions.Data
{
    public interface IOrderBy
    {
        /// <summary>
        /// Direction of this order by. 
        /// </summary>
        OrderByType OrderByType { get; }

        /// <summary>
        /// Expression specifying how the sorting is done.
        /// </summary>
        IPropertyMetadata OrderByExpression { get; }

        /// <summary>
        /// Property to which this order by is applied (when used in Editor).
        /// </summary>
        IPropertyMetadata? Property { get; }

        /// <summary>
        /// Expression to which this order by is applied (when used in View).
        /// </summary>
        IExpressionMetadata? Expression { get; }
    }
}
