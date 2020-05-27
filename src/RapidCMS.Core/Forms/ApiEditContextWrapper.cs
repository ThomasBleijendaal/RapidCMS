using System;
using System.Linq;
using System.Linq.Expressions;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Forms;
using RapidCMS.Core.Abstractions.Metadata;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Helpers;

namespace RapidCMS.Core.Forms
{
    public class ApiEditContextWrapper<TEntity> : IEditContext<TEntity>
        where TEntity : class, IEntity
    {
        private readonly TEntity? _currentEntity;
        private readonly FormState _formState;

        public ApiEditContextWrapper(
            UsageType usageType, 
            EntityState entityState, 
            TEntity entity, 
            TEntity? currentEntity,
            IParent? parent,
            IServiceProvider serviceProvider)
        {
            UsageType = usageType;
            EntityState = entityState;
            Entity = entity;
            _currentEntity = currentEntity;
            Parent = parent;

            _formState = new FormState(entity, serviceProvider);
        }

        public UsageType UsageType { get; }
        public EntityState EntityState { get; }
        public TEntity Entity { get; }
        public IParent? Parent { get; }

        public IRelationContainer GetRelationContainer()
        {
            // TODO TODO TODO
            return new RelationContainer(Enumerable.Empty<IRelation>());
        }
         
        public bool? IsModified<TValue>(Expression<Func<TEntity, TValue>> property)
        {
            if (_currentEntity == null)
            {
                return true;
            }

            var metadata = PropertyMetadataHelper.GetPropertyMetadata(property);

            return !metadata?.Getter(Entity)?.Equals(_currentEntity);
        }

        public bool? IsModified(string propertyName)
        {
            var property = typeof(TEntity).GetProperty(propertyName);

            return !property?.GetValue(Entity)?.Equals(property.GetValue(_currentEntity));
        }

        public bool? IsValid<TValue>(Expression<Func<TEntity, TValue>> property) 
            => _formState.GetPropertyState(GetMetadata(property))?.GetValidationMessages().Any();

        public bool? IsValid(string propertyName) 
            => _formState.GetPropertyState(propertyName)?.GetValidationMessages().Any();

        public bool? WasValidated<TValue>(Expression<Func<TEntity, TValue>> property) 
            => _formState.GetPropertyState(GetMetadata(property))?.WasValidated;

        public bool? WasValidated(string propertyName) 
            => _formState.GetPropertyState(propertyName)?.WasValidated;

        private IPropertyMetadata GetMetadata<TValue>(Expression<Func<TEntity, TValue>> property) 
            => PropertyMetadataHelper.GetPropertyMetadata(property) ?? throw new InvalidOperationException("Given expression cannot be converted to PropertyMetadata");
    }
}
