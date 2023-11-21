using RapidCMS.Core.Abstractions.Metadata;

namespace RapidCMS.Core.Abstractions.Data;

internal interface IDataValidationProvider
{
    IPropertyMetadata Property { get; }
}
