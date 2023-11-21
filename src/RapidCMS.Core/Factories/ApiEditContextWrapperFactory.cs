using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Factories;
using RapidCMS.Core.Abstractions.Forms;
using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Extensions;
using RapidCMS.Core.Forms;
using RapidCMS.Core.Helpers;
using RapidCMS.Core.Models.Data;
using RapidCMS.Core.Models.Setup;

namespace RapidCMS.Core.Factories;

internal class ApiEditContextWrapperFactory : IEditContextFactory
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ISetupResolver<EntityVariantSetup> _entityVariantResolver;
    private readonly ISetupResolver<IReadOnlyList<ValidationSetup>> _validationResolver;

    public ApiEditContextWrapperFactory(
        IServiceProvider serviceProvider,
        ISetupResolver<EntityVariantSetup> entityVariantResolver,
        ISetupResolver<IReadOnlyList<ValidationSetup>> validationResolver)
    {
        _serviceProvider = serviceProvider;
        _entityVariantResolver = entityVariantResolver;
        _validationResolver = validationResolver;
    }

    public Task<IEditContext> GetEditContextWrapperAsync(FormEditContext editContext)
    {
        var contextType = typeof(FormEditContextWrapper<>).MakeGenericType(editContext.Entity.GetType());
        var instance = Activator.CreateInstance(contextType, editContext);

        return Task.FromResult<IEditContext>(instance as IEditContext
            ?? throw new InvalidOperationException("Cannot create FormEditContextWrapper"));
    }

    public async Task<IEditContext> GetEditContextWrapperAsync(
        UsageType usageType,
        EntityState entityState,
        Type repositoryEntityType,
        IEntity updatedEntity,
        IEntity referenceEntity,
        IParent? parent,
        IEnumerable<(string propertyName, string typeName, IEnumerable<object> elements)> relations)
    {
        var container = new RelationContainer(await relations.SelectNotNullAsync(async r =>
        {
            var variant = await _entityVariantResolver.ResolveSetupAsync(r.typeName);
            var property = PropertyMetadataHelper.GetPropertyMetadata(updatedEntity.GetType(), r.propertyName);
            if (property == null)
            {
                return null;
            }

            return new Relation(variant.Type, property, r.elements.ToList());
        }).ToListAsync());

        var entityVariantAlias = AliasHelper.GetEntityVariantAlias(updatedEntity.GetType());
        var validations = await _validationResolver.ResolveSetupAsync(entityVariantAlias);

        var contextType = typeof(ApiEditContextWrapper<>).MakeGenericType(repositoryEntityType);
        var instance = Activator.CreateInstance(contextType,
            usageType,
            entityState,
            updatedEntity,
            referenceEntity,
            parent,
            container,
            validations,
            _serviceProvider);

        return instance as IEditContext ?? throw new InvalidOperationException("Cannot create ApiEditContextWrapper");
    }
}
