using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Metadata;
using RapidCMS.Core.Abstractions.Validators;
using RapidCMS.Core.Validators;

namespace RapidCMS.Core.Providers
{
    internal class ApiDataProvider : IDataValidationProvider
    {
        private IRelationValidator? _validator { get; set; }
        private IReadOnlyList<object> _relatedElementIds { get; set; }

        public ApiDataProvider(IRelation relation)
        {
            _validator =  new RelationValidationAttributeValidator(relation.Property);
            Property = relation.Property;
            _relatedElementIds = relation.RelatedElementIds;
        }

        public IPropertyMetadata Property { get; private set; }

        public IEnumerable<ValidationResult> Validate(IEntity entity, IServiceProvider serviceProvider)
        {
            if (_validator != null)
            {
                return _validator.Validate(entity, _relatedElementIds, serviceProvider);
            }
            else
            {
                return Enumerable.Empty<ValidationResult>();
            }
        }
    }
}
