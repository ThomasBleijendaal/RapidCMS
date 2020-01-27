using System.Linq.Expressions;

namespace RapidCMS.Core.Abstractions.Data
{
    public interface IDataView
    {
        /// <summary>
        /// Id of the data view.
        /// </summary>
        int Id { get; }

        /// <summary>
        /// Display label of the data view.
        /// </summary>
        string Label { get; }

        /// <summary>
        /// Associated query expression of this data view.
        /// </summary>
        LambdaExpression QueryExpression { get; }
    }
}
