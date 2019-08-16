using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using RapidCMS.Common.Models.Metadata;

namespace RapidCMS.Common.Data
{
    public class DataProvider
    {
        public DataProvider(IPropertyMetadata property, IDataCollection collection, IRelationValidator? validator)
        {
            Property = property ?? throw new ArgumentNullException(nameof(property));
            Collection = collection ?? throw new ArgumentNullException(nameof(collection));
            Validator = validator;
        }

        public IPropertyMetadata Property { get; private set; }
        public IDataCollection Collection { get; private set; }
        private IRelationValidator? Validator { get; set; }

        public IEnumerable<ValidationResult> Validate(IEntity entity)
        {
            if (Validator != null && Collection is IRelationDataCollection relationDataCollection)
            {
                return Validator.Validate(entity, relationDataCollection.GetCurrentRelatedElements());
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
