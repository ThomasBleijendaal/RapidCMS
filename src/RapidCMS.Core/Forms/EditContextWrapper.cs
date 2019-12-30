using System;
using System.Linq;
using System.Linq.Expressions;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Extensions;
using RapidCMS.Core.Helpers;
using RapidCMS.Core.Interfaces.Data;
using RapidCMS.Core.Interfaces.Forms;
using RapidCMS.Core.Interfaces.Metadata;

namespace RapidCMS.Core.Forms
{
    public class EditContextWrapper<TEntity> : IEditContext<TEntity>
        where TEntity : IEntity
    {
        private readonly EditContext _editContext;

        public EditContextWrapper(EditContext editContext)
        {
            _editContext = editContext;
        }

        public UsageType UsageType => _editContext.UsageType;

        public EntityState EntityState => _editContext.EntityState;

        public TEntity Entity => (TEntity)_editContext.Entity;

        public IParent? Parent => _editContext.Parent;

        public IRelationContainer GetRelationContainer()
        {
            // TODO
            return default;
             //return new RelationContainer(_editContext.DataProviders.Select(x => x.GenerateRelation()).SelectNotNull(x => x as IRelation));
        }

        public bool? IsModified<TValue>(Expression<Func<TEntity, TValue>> property)
        {
            return GetPropertyState(GetMetadata(property))?.IsModified;
        }

        public bool? IsModified(string propertyName)
        {
            return GetPropertyState(propertyName)?.IsModified;
        }

        public bool? IsValid<TValue>(Expression<Func<TEntity, TValue>> property)
        {
            return GetPropertyState(GetMetadata(property))?.GetValidationMessages().Any() == false;
        }

        public bool? IsValid(string propertyName)
        {
            return GetPropertyState(propertyName)?.GetValidationMessages().Any() == false;
        }

        public bool? WasValidated<TValue>(Expression<Func<TEntity, TValue>> property)
        {
            return GetPropertyState(GetMetadata(property))?.WasValidated;
        }

        public bool? WasValidated(string propertyName)
        {
            return GetPropertyState(propertyName)?.WasValidated;
        }

        private IPropertyMetadata GetMetadata<TValue>(Expression<Func<TEntity, TValue>> property)
        {
            return PropertyMetadataHelper.GetPropertyMetadata(property) ?? throw new InvalidOperationException("Given expression cannot be converted to PropertyMetadata");
        }

        internal PropertyState? GetPropertyState(IPropertyMetadata property)
        {
            return _editContext.GetPropertyState(property, false);
        }
        internal PropertyState? GetPropertyState(string propertyName)
        {
            return _editContext.GetPropertyState(propertyName);
        }
    }
}
