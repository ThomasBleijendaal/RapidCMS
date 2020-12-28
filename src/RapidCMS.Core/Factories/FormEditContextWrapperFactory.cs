using System;
using System.Collections.Generic;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Factories;
using RapidCMS.Core.Abstractions.Forms;
using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Abstractions.Setup;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Forms;

namespace RapidCMS.Core.Factories
{
    internal class FormEditContextWrapperFactory : IEditContextFactory
    {
        private readonly ISetupResolver<ICollectionSetup> _collectionResolver;

        public FormEditContextWrapperFactory(ISetupResolver<ICollectionSetup> collectionResolver)
        {
            _collectionResolver = collectionResolver;
        }

        public IEditContext GetEditContextWrapper(FormEditContext editContext)
        {
            var collection = _collectionResolver.ResolveSetup(editContext.CollectionAlias);

            var contextType = typeof(FormEditContextWrapper<>).MakeGenericType(collection.EntityVariant.Type);
            var instance = Activator.CreateInstance(contextType, editContext);

            return instance as IEditContext ?? throw new InvalidOperationException("Cannot create FormEditContextWrapper");
        }

        public IEditContext GetEditContextWrapper(
            UsageType usageType,
            EntityState entityState,
            IEntity updatedEntity,
            IEntity referenceEntity,
            IParent? parent,
            IEnumerable<(string propertyName, string typeName, IEnumerable<object> elements)> relations)
        {
            throw new NotImplementedException();
        }
    }
}
