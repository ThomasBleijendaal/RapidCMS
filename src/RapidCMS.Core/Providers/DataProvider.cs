using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Metadata;
using RapidCMS.Core.Abstractions.Validators;
using RapidCMS.Core.Models.Data;

namespace RapidCMS.Core.Providers
{
    internal class DataProvider
    {
        internal DataProvider(IPropertyMetadata property, IDataCollection collection, IRelationValidator? validator)
        {
            Property = property ?? throw new ArgumentNullException(nameof(property));
            Collection = collection ?? throw new ArgumentNullException(nameof(collection));
            Validator = validator;
        }

        public IPropertyMetadata Property { get; private set; }
        public IDataCollection Collection { get; private set; }
        private IRelationValidator? Validator { get; set; }

        public IEnumerable<ValidationResult> Validate(IEntity entity, IServiceProvider serviceProvider)
        {
            if (Validator != null && Collection is IRelationDataCollection relationDataCollection)
            {
                return Validator.Validate(entity, relationDataCollection.GetCurrentRelatedElements(), serviceProvider);
            }
            else
            {
                return Enumerable.Empty<ValidationResult>();
            }
        }

        internal IRelation? GenerateRelation()
        {
            if (Collection is IRelationDataCollection relationDataCollection)
            {
                return new Relation(relationDataCollection.GetRelatedEntityType(), Property, relationDataCollection.GetCurrentRelatedElements());
            }
            else
            {
                return default;
            }
        }
    }
}
