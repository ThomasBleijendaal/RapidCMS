using RapidCMS.Core.Enums;
using RapidCMS.Core.Interfaces.Metadata;

namespace RapidCMS.Core.Interfaces.Data
{
    public interface IOrderBy
    {
        OrderByType OrderByType { get; }
        IPropertyMetadata OrderByExpression { get; }
    }
}
