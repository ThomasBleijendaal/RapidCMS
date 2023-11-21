using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Factories;
using RapidCMS.Core.Abstractions.Forms;
using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Forms;
using RapidCMS.Core.Models.Setup;

namespace RapidCMS.Core.Factories;

internal class FormEditContextWrapperFactory : IEditContextFactory
{
    private readonly ISetupResolver<CollectionSetup> _collectionResolver;

    public FormEditContextWrapperFactory(ISetupResolver<CollectionSetup> collectionResolver)
    {
        _collectionResolver = collectionResolver;
    }

    public async Task<IEditContext> GetEditContextWrapperAsync(FormEditContext editContext)
    {
        var collection = await _collectionResolver.ResolveSetupAsync(editContext.CollectionAlias);

        var contextType = typeof(FormEditContextWrapper<>).MakeGenericType(collection.EntityVariant.Type);
        var instance = Activator.CreateInstance(contextType, editContext);

        return instance as IEditContext ?? throw new InvalidOperationException("Cannot create FormEditContextWrapper");
    }

    public Task<IEditContext> GetEditContextWrapperAsync(
        UsageType usageType,
        EntityState entityState,
        Type repositoryEntityType,
        IEntity updatedEntity,
        IEntity referenceEntity,
        IParent? parent,
        IEnumerable<(string propertyName, string typeName, IEnumerable<object> elements)> relations)
    {
        throw new NotImplementedException();
    }
}
