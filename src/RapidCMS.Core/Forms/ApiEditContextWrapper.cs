using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Forms;
using RapidCMS.Core.Abstractions.Metadata;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Exceptions;
using RapidCMS.Core.Helpers;
using RapidCMS.Core.Models.Setup;

namespace RapidCMS.Core.Forms;

public class ApiEditContextWrapper<TEntity> : IEditContext<TEntity>
    where TEntity : class, IEntity
{
    private readonly FormState _formState;
    private readonly IRelationContainer _relationContainer;

    public ApiEditContextWrapper(
        UsageType usageType,
        EntityState entityState,
        TEntity entity,
        TEntity referenceEntity,
        IParent? parent,
        IRelationContainer relationContainer,
        IReadOnlyList<ValidationSetup> validators,
        IServiceProvider serviceProvider)
    {
        UsageType = usageType;
        EntityState = entityState;
        Entity = entity;
        Parent = parent;
        _relationContainer = relationContainer;
        _formState = new FormState(entity, validators, serviceProvider);
        _formState.PopulatePropertyStatesUsingReferenceEntity(referenceEntity);
    }

    public UsageType UsageType { get; }
    public EntityState EntityState { get; }
    public TEntity Entity { get; }
    public IParent? Parent { get; }
    public string CollectionAlias => throw new NotSupportedException("The collection alias is unknown in API contexts");

    public ModelStateDictionary ValidationErrors => _formState.ModelState;

    public IRelationContainer GetRelationContainer() => _relationContainer;

    public bool? IsModified<TValue>(Expression<Func<TEntity, TValue>> property)
        => GetPropertyState(GetMetadata(property))?.IsModified;

    public bool? IsModified(string propertyName)
        => GetPropertyState(propertyName)?.IsModified;

    public async Task<bool> IsValidAsync()
    {
        await _formState.ValidateModelAsync(_relationContainer);

        return !_formState.GetValidationMessages().Any();
    }

    public bool? IsValid<TValue>(Expression<Func<TEntity, TValue>> property)
        => GetPropertyState(GetMetadata(property))?.GetValidationMessages().Any();

    public bool? IsValid(string propertyName)
        => GetPropertyState(propertyName)?.GetValidationMessages().Any();

    public bool? WasValidated<TValue>(Expression<Func<TEntity, TValue>> property)
        => GetPropertyState(GetMetadata(property))?.WasValidated;

    public bool? WasValidated(string propertyName)
        => GetPropertyState(propertyName)?.WasValidated;

    public void AddValidationError<TValue>(Expression<Func<TEntity, TValue>> property, string message)
        => GetPropertyState(GetMetadata(property))?.AddMessage(message);

    public void AddValidationError(string propertyName, string message)
        => GetPropertyState(propertyName)?.AddMessage(message);

    public async Task<bool?> ValidateAsync<TValue>(Expression<Func<TEntity, TValue>> property)
    {
        var metadata = GetMetadata(property);

        // force add property to the formState
        _formState.GetPropertyState(metadata, createWhenNotFound: true);

        await _formState.ValidatePropertyAsync(metadata, _relationContainer);

        return _formState.GetPropertyState(metadata)?.GetValidationMessages().Any()
            ?? throw new InvalidOperationException("Given expression could not be validated.");
    }

    public async Task EnforceCompleteValidationAsync()
    {
        // add all properties to the form state
        _formState.PopulateAllPropertyStates();

        await EnforceValidEntityAsync();
    }

    public async Task EnforceValidEntityAsync()
    {
        if (!await IsValidAsync())
        {
            throw new InvalidEntityException();
        }
    }

    private PropertyState? GetPropertyState(IPropertyMetadata property)
        => _formState.GetPropertyState(property, false);

    private PropertyState? GetPropertyState(string propertyName)
        => _formState.GetPropertyState(propertyName);

    private IPropertyMetadata GetMetadata<TValue>(Expression<Func<TEntity, TValue>> property)
        => PropertyMetadataHelper.GetPropertyMetadata(property)
        ?? throw new InvalidOperationException("Given expression cannot be converted to PropertyMetadata");
}
