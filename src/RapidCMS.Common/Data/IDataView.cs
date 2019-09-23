using System.Linq.Expressions;

namespace RapidCMS.Common.Data
{
    public interface IDataView
    {
        int Id { get; }
        string Label { get; }
        LambdaExpression QueryExpression { get; }
    }
}
