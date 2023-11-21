using System.Collections.Generic;
using System.Linq.Expressions;
using RapidCMS.Core.Abstractions.Metadata;
using RapidCMS.Core.Enums;

namespace RapidCMS.Core.Abstractions.Data;

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

    /// <summary>
    /// Default sorts for this data view.
    /// </summary>
    IEnumerable<KeyValuePair<IPropertyMetadata, OrderByType>> DefaultOrderBys { get; }
}
