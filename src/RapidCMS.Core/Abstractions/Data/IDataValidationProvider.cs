using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using RapidCMS.Core.Abstractions.Metadata;

namespace RapidCMS.Core.Abstractions.Data
{
    [Obsolete]
    internal interface IDataValidationProvider
    {
        IPropertyMetadata Property { get; }

        IEnumerable<ValidationResult> Validate(IEntity entity, IServiceProvider serviceProvider);
    }
}
