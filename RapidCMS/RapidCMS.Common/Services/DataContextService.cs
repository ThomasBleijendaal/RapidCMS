using System;
using RapidCMS.Common.Data;
using RapidCMS.Common.Extensions;
using RapidCMS.Common.Models;

namespace RapidCMS.Common.Services
{
    internal interface IDataProviderService
    {
        DataProvider? GetDataProvider(Field field);
    }

    internal class DataProviderService : IDataProviderService
    {
        private readonly Root _root;
        private readonly IServiceProvider _serviceProvider;

        public DataProviderService(Root root, IServiceProvider serviceProvider)
        {
            _root = root;
            _serviceProvider = serviceProvider;
        }

        public DataProvider? GetDataProvider(Field field)
        {
            if (!(field is PropertyField propertyField && propertyField.Relation != null))
            {
                return null;
            }

            switch (propertyField.Relation)
            {
                case CollectionRelation collectionRelation:

                    var repo = _root.GetRepository(collectionRelation.CollectionAlias);
                    if (repo == null)
                    {
                        throw new InvalidOperationException($"Field {propertyField.Property.PropertyName} has incorrectly configure relation, cannot find repository for collection alias {collectionRelation.CollectionAlias}.");
                    }

                    var provider = new CollectionDataProvider(
                        repo,
                        collectionRelation.RelatedEntityType,
                        collectionRelation.RepositoryParentIdProperty,
                        collectionRelation.IdProperty,
                        collectionRelation.DisplayProperties);

                    var validator = collectionRelation.ValidationFunction != null
                        ? new CollectionDataValidator(propertyField.Property, collectionRelation.ValidationFunction)
                        : default;

                    return new DataProvider(propertyField.Property, provider, validator);

                case DataProviderRelation dataProviderRelation:

                    return new DataProvider(propertyField.Property, _serviceProvider.GetService<IDataCollection>(dataProviderRelation.DataCollectionType), default);

                default:
                    throw new InvalidOperationException();
            };
        }
    }


}
