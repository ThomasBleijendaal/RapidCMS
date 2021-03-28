using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Forms;
using RapidCMS.Core.Abstractions.Metadata;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Exceptions;
using RapidCMS.Core.Helpers;
using RapidCMS.Core.Providers;

namespace RapidCMS.Core.Forms
{
    public class ApiEditContextWrapper<TEntity> : IEditContext<TEntity>
        where TEntity : class, IEntity
    {
        private readonly FormState _formState;
        private readonly IRelationContainer _relationContainer;
        private readonly IEnumerable<IDataValidationProvider> _dataValidationProviders;

        public ApiEditContextWrapper(
            UsageType usageType,
            EntityState entityState,
            TEntity entity,
            TEntity referenceEntity,
            IParent? parent,
            IRelationContainer relationContainer,
            IServiceProvider serviceProvider)
        {
            UsageType = usageType;
            EntityState = entityState;
            Entity = entity;
            Parent = parent;
            _relationContainer = relationContainer;
            _formState = new FormState(entity, serviceProvider);
            _formState.PopulatePropertyStatesUsingReferenceEntity(referenceEntity);

            _dataValidationProviders = _relationContainer.Relations.Select(relation => new ApiDataProvider(relation)).ToList();
        }

        public UsageType UsageType { get; }
        public EntityState EntityState { get; }
        public TEntity Entity { get; }
        public IParent? Parent { get; }

        public ModelStateDictionary ValidationErrors => _formState.ModelState;

        public IRelationContainer GetRelationContainer() => _relationContainer;

        public bool? IsModified<TValue>(Expression<Func<TEntity, TValue>> property)
            => GetPropertyState(GetMetadata(property))?.IsModified;

        public bool? IsModified(string propertyName)
            => GetPropertyState(propertyName)?.IsModified;

        public bool IsValid()
        {
            _formState.ValidateModel(_dataValidationProviders);

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

        public bool? Validate<TValue>(Expression<Func<TEntity, TValue>> property)
        {
            var metadata = GetMetadata(property);

            // force add property to the formState
            _formState.GetPropertyState(metadata, createWhenNotFound: true);

            _formState.ValidateProperty(metadata, _dataValidationProviders);

            return _formState.GetPropertyState(metadata)?.GetValidationMessages().Any()
                ?? throw new InvalidOperationException("Given expression could not be valided.");
        }

        public void EnforceCompleteValidation()
        {
            // add all properties to the form state
            _formState.PopulateAllPropertyStates();

            if (!IsValid())
            {
                throw new InvalidEntityException();
            }
        }

        public void EnforceValidEntity()
        {
            if (!IsValid())
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
}
