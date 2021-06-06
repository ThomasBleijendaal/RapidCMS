﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Components.Forms;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Metadata;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Providers;

namespace RapidCMS.Core.Forms
{
    public sealed class FormEditContext
    {
        internal FormEditContext(
            string collectionAlias,
            string repositoryAlias,
            string entityVariantAlias,
            IEntity entity,
            IParent? parent,
            UsageType usageType,
            IServiceProvider serviceProvider)
        {
            CollectionAlias = collectionAlias ?? throw new ArgumentNullException(nameof(collectionAlias));
            RepositoryAlias = repositoryAlias ?? throw new ArgumentNullException(nameof(repositoryAlias));
            EntityVariantAlias = entityVariantAlias ?? throw new ArgumentNullException(nameof(entityVariantAlias));
            Entity = entity ?? throw new ArgumentNullException(nameof(entity));
            Parent = parent;
            UsageType = usageType;

            FormState = new FormState(Entity, serviceProvider);
        }

        internal FormEditContext(
            FormEditContext protoEditContext,
            IEntity entity,
            UsageType usageType,
            IServiceProvider serviceProvider)
        {
            CollectionAlias = protoEditContext.CollectionAlias;
            RepositoryAlias = protoEditContext.RepositoryAlias;
            EntityVariantAlias = protoEditContext.EntityVariantAlias;
            Entity = entity ?? throw new ArgumentNullException(nameof(entity));
            Parent = protoEditContext.Parent;
            UsageType = usageType;

            FormState = new FormState(Entity, serviceProvider);
        }

        internal readonly FormState FormState;

        public string CollectionAlias { get; private set; }
        public string RepositoryAlias { get; private set; }
        public string EntityVariantAlias { get; private set; }
        public IEntity Entity { get; private set; }
        public IParent? Parent { get; private set; }
        public UsageType UsageType { get; private set; }
         
        public ReorderedState ReorderedState { get; private set; }
        internal string? ReorderedBeforeId { get; private set; }
        public EntityState EntityState => UsageType.HasFlag(UsageType.New) ? EntityState.IsNew : EntityState.IsExisting;

        internal List<FormDataProvider> DataProviders { get; set; } = new List<FormDataProvider>();

        public event EventHandler<FieldChangedEventArgs>? OnFieldChanged;

        public event EventHandler<ValidationStateChangedEventArgs>? OnValidationStateChanged;

        public static implicit operator EditContext(FormEditContext editContext) 
            => new EditContext(editContext.Entity);

        public FormEditContext EntityProperty(IPropertyMetadata property) 
            => new FormEditContext(
                CollectionAlias,
                RepositoryAlias,
                EntityVariantAlias,
                (IEntity)property.Getter(Entity),
                default,
                UsageType.Edit | UsageType.Node,
                FormState.ServiceProvider);

        public void NotifyReordered(string? beforeId)
        {
            ReorderedState = ReorderedState.Reordered;
            ReorderedBeforeId = beforeId;
        }

        public void NotifyPropertyIncludedInForm(IPropertyMetadata property)
        {
            GetPropertyState(property);
        }

        public void NotifyPropertyChanged(IPropertyMetadata property)
        {
            ValidateProperty(property);

            GetPropertyState(property)!.IsModified = true;
            OnFieldChanged?.Invoke(this, new FieldChangedEventArgs(property));
        }

        public void NotifyPropertyBusy(IPropertyMetadata property)
        {
            GetPropertyState(property)!.IsBusy = true;
            OnValidationStateChanged?.Invoke(this, new ValidationStateChangedEventArgs(false));
        }

        public void NotifyPropertyFinished(IPropertyMetadata property)
        {
            GetPropertyState(property)!.IsBusy = false;
            OnValidationStateChanged?.Invoke(this, new ValidationStateChangedEventArgs());
        }

        public bool IsValid()
        {
            ValidateModel();

            return !FormState.GetValidationMessages().Any();
        }

        public bool IsModified() 
            => FormState.GetPropertyStates().Any(x => x.IsModified);

        public bool IsModified(IPropertyMetadata property) 
            => GetPropertyState(property)!.IsModified;

        public bool IsReordered() 
            => ReorderedState == ReorderedState.Reordered;

        public bool IsValid(IPropertyMetadata property) 
            => !GetPropertyState(property)!.GetValidationMessages().Any();

        public bool WasValidated(IPropertyMetadata property)
            => GetPropertyState(property)!.WasValidated;

        public void AddValidationMessage(IPropertyMetadata property, string message)
        {
            GetPropertyState(property, true)!.AddMessage(message);
            GetPropertyState(property, true)!.WasValidated = true;
        }

        public IEnumerable<string> GetValidationMessages(IPropertyMetadata property)
        {
            var state = GetPropertyState(property, false);
            if (state != null)
            {
                foreach (var message in state.GetValidationMessages())
                {
                    yield return message;
                }
            }
        }

        public IEnumerable<string> GetStrayValidationMessages()
            => FormState.GetStrayValidationMessages();

        internal PropertyState? GetPropertyState(IPropertyMetadata property, bool createWhenNotFound = true)
            => FormState.GetPropertyState(property, createWhenNotFound);

        internal PropertyState? GetPropertyState(string propertyName)
            => FormState.GetPropertyState(propertyName);

        private void ValidateModel()
        {
            FormState.ValidateModel(DataProviders);

            OnValidationStateChanged?.Invoke(this, new ValidationStateChangedEventArgs(isValid: !FormState.GetValidationMessages().Any()));
        }

        private void ValidateProperty(IPropertyMetadata property)
        {
            FormState.ValidateProperty(property, DataProviders);

            OnValidationStateChanged?.Invoke(this, new ValidationStateChangedEventArgs(isValid: !FormState.GetValidationMessages().Any()));
        }
    }
}
