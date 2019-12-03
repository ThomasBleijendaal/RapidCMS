using RapidCMS.Common.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RapidCMS.Common.Forms.Validation
{
    public abstract class RelationValidationAttribute : Attribute
    {
        public abstract ValidationResult? IsValid(IEntity entity, IEnumerable<IElement> relatedElements, ValidationContext validationContext);
    }
}
