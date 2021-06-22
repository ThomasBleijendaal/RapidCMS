using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using RapidCMS.Core.Abstractions.Metadata;

namespace RapidCMS.Core.Abstractions.Data
{
    internal interface IDataValidationProvider
    {
        IPropertyMetadata Property { get; }

        [Obsolete]
        IEnumerable<ValidationResult> Validate(IEntity entity, IServiceProvider serviceProvider);
    }
}
