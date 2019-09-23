using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using RapidCMS.Common.Attributes;
using RapidCMS.Common.Extensions;
using RapidCMS.Common.Models.Metadata;

namespace RapidCMS.Common.Data
{
    internal class CollectionDataValidator : IRelationValidator
    {
        private readonly IPropertyMetadata _property;

        public CollectionDataValidator(IPropertyMetadata property)
        {
            _property = property;
        }

        public IEnumerable<ValidationResult> Validate(IEntity entity, IEnumerable<IElement> relatedElements, IServiceProvider serviceProvider)
        {
            var validationAttributes = _property.GetAttributes<RelationValidationAttribute>();
            var validationContext = new ValidationContext(entity, serviceProvider, default)
            {
                MemberName = _property.PropertyName
            };

            return validationAttributes
                .Select(attr => attr.IsValid(entity, relatedElements, validationContext))
                .WhereAs(result => result as ValidationResult);
        }
    }
}
