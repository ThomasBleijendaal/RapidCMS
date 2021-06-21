using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using RapidCMS.Core.Abstractions.Data;

namespace RapidCMS.Core.Forms.Validation
{
    [Obsolete]
    public abstract class RelationValidationAttribute : Attribute
    {
        public abstract ValidationResult? IsValid(IEntity entity, IReadOnlyList<object> relatedElementIds, ValidationContext validationContext);
    }
}
