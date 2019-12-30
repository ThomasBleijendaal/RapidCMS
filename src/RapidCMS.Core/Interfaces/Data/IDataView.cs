using System.Linq.Expressions;

namespace RapidCMS.Core.Interfaces.Data
{
    public interface IDataView
    {
        int Id { get; }
        string Label { get; }
        LambdaExpression QueryExpression { get; }
    }
}
