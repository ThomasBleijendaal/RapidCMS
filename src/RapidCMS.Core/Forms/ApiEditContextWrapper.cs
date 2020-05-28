using System;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Forms;
using RapidCMS.Core.Abstractions.Metadata;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Exceptions;
using RapidCMS.Core.Helpers;

namespace RapidCMS.Core.Forms
{
    public class ApiEditContextWrapper<TEntity> : IEditContext<TEntity>
        where TEntity : class, IEntity
    {
        private readonly FormState _formState;

        public ApiEditContextWrapper(
            UsageType usageType,
            EntityState entityState,
            TEntity entity,
            TEntity referenceEntity,
            IParent? parent,
            IServiceProvider serviceProvider)
        {
            UsageType = usageType;
            EntityState = entityState;
            Entity = entity;
            Parent = parent;

            _formState = new FormState(entity, serviceProvider);
            _formState.PopulatePropertyStatesUsingReferenceEntity(referenceEntity);
        }

        public UsageType UsageType { get; }
        public EntityState EntityState { get; }
        public TEntity Entity { get; }
        public IParent? Parent { get; }

        public ModelStateDictionary ValidationErrors => _formState.ModelState;

        public IRelationContainer GetRelationContainer()
        {
            // TODO TODO TODO
            return new RelationContainer(Enumerable.Empty<IRelation>());
        }

        public bool? IsModified<TValue>(Expression<Func<TEntity, TValue>> property)
            => GetPropertyState(GetMetadata(property))?.IsModified;

        public bool? IsModified(string propertyName)
            => GetPropertyState(propertyName)?.IsModified;

        public bool IsValid()
        {
            _formState.ValidateModel();

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

        public bool? Validate<TValue>(Expression<Func<TEntity, TValue>> property)
        {
            var metadata = GetMetadata(property);

            // force add property to the formState
            _formState.GetPropertyState(metadata, createWhenNotFound: true);

            _formState.ValidateProperty(metadata);

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

        private PropertyState? GetPropertyState(IPropertyMetadata property)
            => _formState.GetPropertyState(property, false);

        private PropertyState? GetPropertyState(string propertyName)
            => _formState.GetPropertyState(propertyName);

        private IPropertyMetadata GetMetadata<TValue>(Expression<Func<TEntity, TValue>> property)
            => PropertyMetadataHelper.GetPropertyMetadata(property)
            ?? throw new InvalidOperationException("Given expression cannot be converted to PropertyMetadata");
    }
}
