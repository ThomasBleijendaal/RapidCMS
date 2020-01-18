using RapidCMS.Core.Abstractions.Metadata;
using RapidCMS.Core.Enums;

namespace RapidCMS.Core.Abstractions.Data
{
    public interface IOrderBy
    {
        OrderByType OrderByType { get; }
        IPropertyMetadata OrderByExpression { get; }
    }
}
