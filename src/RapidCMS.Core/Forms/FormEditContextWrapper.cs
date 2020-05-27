using System;
using System.Linq;
using System.Linq.Expressions;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Forms;
using RapidCMS.Core.Abstractions.Metadata;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Extensions;
using RapidCMS.Core.Helpers;

namespace RapidCMS.Core.Forms
{
    public class FormEditContextWrapper<TEntity> : IEditContext<TEntity>
        where TEntity : IEntity
    {
        private readonly EditContext _editContext;

        public FormEditContextWrapper(EditContext editContext)
        {
            _editContext = editContext;
        }

        public UsageType UsageType => _editContext.UsageType;

        public EntityState EntityState => _editContext.EntityState;

        public TEntity Entity => (TEntity)_editContext.Entity;

        public IParent? Parent => _editContext.Parent;

        public IRelationContainer GetRelationContainer() 
            => new RelationContainer(_editContext.DataProviders.Select(x => x.GenerateRelation()).SelectNotNull(x => x));

        public bool? IsModified<TValue>(Expression<Func<TEntity, TValue>> property) 
            => GetPropertyState(GetMetadata(property))?.IsModified;

        public bool? IsModified(string propertyName) 
            => GetPropertyState(propertyName)?.IsModified;

        public bool? IsValid<TValue>(Expression<Func<TEntity, TValue>> property) 
            => GetPropertyState(GetMetadata(property))?.GetValidationMessages().Any() == false;

        public bool? IsValid(string propertyName)
            => GetPropertyState(propertyName)?.GetValidationMessages().Any() == false;

        public bool? WasValidated<TValue>(Expression<Func<TEntity, TValue>> property) 
            => GetPropertyState(GetMetadata(property))?.WasValidated;

        public bool? WasValidated(string propertyName) 
            => GetPropertyState(propertyName)?.WasValidated;

        internal PropertyState? GetPropertyState(IPropertyMetadata property) 
            => _editContext.GetPropertyState(property, false);

        internal PropertyState? GetPropertyState(string propertyName)
            => _editContext.GetPropertyState(propertyName);

        private IPropertyMetadata GetMetadata<TValue>(Expression<Func<TEntity, TValue>> property)
            => PropertyMetadataHelper.GetPropertyMetadata(property) ?? throw new InvalidOperationException("Given expression cannot be converted to PropertyMetadata");
    }
}
