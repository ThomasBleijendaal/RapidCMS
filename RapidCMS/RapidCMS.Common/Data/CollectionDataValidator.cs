using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using RapidCMS.Common.Models.Metadata;

namespace RapidCMS.Common.Data
{
    internal class CollectionDataValidator : IRelationValidator
    {
        private readonly IPropertyMetadata _property;
        private readonly Func<IEntity, IEnumerable<IElement>, IEnumerable<string>?> _validationFunction;

        public CollectionDataValidator(IPropertyMetadata property, Func<IEntity, IEnumerable<IElement>, IEnumerable<string>?> validationFunction)
        {
            _property = property;
            _validationFunction = validationFunction ?? throw new ArgumentNullException(nameof(validationFunction));
        }

        public IEnumerable<ValidationResult> Validate(IEntity entity, IEnumerable<IElement> relatedElements)
        {
            return _validationFunction.Invoke(entity, relatedElements)?
                .Select(validationMessage => new ValidationResult(validationMessage, new[] { _property.PropertyName }))
                ?? Enumerable.Empty<ValidationResult>();
        }
    }
}
