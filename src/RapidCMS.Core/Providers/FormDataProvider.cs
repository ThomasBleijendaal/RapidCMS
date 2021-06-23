using System;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Metadata;
using RapidCMS.Core.Models.Data;

namespace RapidCMS.Core.Providers
{
    internal class FormDataProvider : IDataValidationProvider
    {
        internal FormDataProvider(IPropertyMetadata property, IDataCollection collection)
        {
            Property = property ?? throw new ArgumentNullException(nameof(property));
            Collection = collection ?? throw new ArgumentNullException(nameof(collection));
        }

        public IPropertyMetadata Property { get; private set; }
        public IDataCollection Collection { get; private set; }

        internal IRelation? GenerateRelation()
        {
            if (Collection is IRelationDataCollection relationDataCollection)
            {
                return new Relation(relationDataCollection.GetRelatedEntityType(), Property, relationDataCollection.GetCurrentRelatedElementIds());
            }
            else
            {
                return default;
            }
        }
    }
}
