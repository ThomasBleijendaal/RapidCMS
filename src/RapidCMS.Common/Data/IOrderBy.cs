using System.Linq.Expressions;
using RapidCMS.Common.Enums;
using RapidCMS.Common.Models.Metadata;

namespace RapidCMS.Common.Data
{
    public interface IOrderBy
    {
        OrderByType OrderByType { get; }
        IPropertyMetadata OrderByExpression { get; }
    }
}
