using System;
using System.Collections.Generic;
using System.Linq;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Factories;
using RapidCMS.Core.Abstractions.Forms;
using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Abstractions.Setup;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Extensions;
using RapidCMS.Core.Forms;
using RapidCMS.Core.Helpers;
using RapidCMS.Core.Models.Data;

namespace RapidCMS.Core.Factories
{
    internal class ApiEditContextWrapperFactory : IEditContextFactory
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ISetupResolver<IEntityVariantSetup> _entityVariantResolver;

        public ApiEditContextWrapperFactory(
            IServiceProvider serviceProvider,
            ISetupResolver<IEntityVariantSetup> entityVariantResolver)
        {
            _serviceProvider = serviceProvider;
            _entityVariantResolver = entityVariantResolver;
        }

        public IEditContext GetEditContextWrapper(FormEditContext editContext)
        {
            var contextType = typeof(FormEditContextWrapper<>).MakeGenericType(editContext.Entity.GetType());
            var instance = Activator.CreateInstance(contextType, editContext);

            return instance as IEditContext ?? throw new InvalidOperationException("Cannot create FormEditContextWrapper");
        }

        public IEditContext GetEditContextWrapper(
            UsageType usageType,
            EntityState entityState,
            Type repositoryEntityType,
            IEntity updatedEntity,
            IEntity referenceEntity,
            IParent? parent,
            IEnumerable<(string propertyName, string typeName, IEnumerable<object> elements)> relations)
        {
            var container = new RelationContainer(relations.SelectNotNull(r =>
            {
                var variant = _entityVariantResolver.ResolveSetup(r.typeName);
                var property = PropertyMetadataHelper.GetPropertyMetadata(updatedEntity.GetType(), r.propertyName);

                if (property == null)
                {
                    return null;
                }

                return new Relation(
                    variant.Type,
                    property,
                    r.elements.Select(e => new Element { Id = e, Labels = Enumerable.Empty<string>() }).ToList());
            }));

            var contextType = typeof(ApiEditContextWrapper<>).MakeGenericType(repositoryEntityType);
            var instance = Activator.CreateInstance(contextType,
                usageType,
                entityState,
                updatedEntity,
                referenceEntity,
                parent,
                container,
                _serviceProvider);

            return instance as IEditContext ?? throw new InvalidOperationException("Cannot create ApiEditContextWrapper");
        }
    }
}
