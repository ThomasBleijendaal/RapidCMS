using System;
using System.Collections.Generic;
using System.Linq;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Metadata;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Providers;

namespace RapidCMS.Core.Forms
{
    public sealed class EditContext
    {
        private readonly FormState _formState;

        internal EditContext(
            string collectionAlias,
            IEntity entity,
            IParent? parent,
            UsageType usageType,
            IServiceProvider serviceProvider)
        {
            CollectionAlias = collectionAlias ?? throw new ArgumentNullException(nameof(collectionAlias));
            Entity = entity ?? throw new ArgumentNullException(nameof(entity));
            Parent = parent;
            UsageType = usageType;

            _formState = new FormState(Entity, serviceProvider);
        }

        public string CollectionAlias { get; private set; }
        public IEntity Entity { get; private set; }
        public IParent? Parent { get; private set; }
        public UsageType UsageType { get; private set; }
         
        public ReorderedState ReorderedState { get; private set; }
        internal string? ReorderedBeforeId { get; private set; }
        public EntityState EntityState => UsageType.HasFlag(UsageType.New) ? EntityState.IsNew : EntityState.IsExisting;

        internal List<DataProvider> DataProviders = new List<DataProvider>();

        public event EventHandler<FieldChangedEventArgs>? OnFieldChanged;

        public event EventHandler<ValidationStateChangedEventArgs>? OnValidationStateChanged;

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

            return !_formState.GetValidationMessages().Any();
        }

        public bool IsModified() 
            => _formState.GetPropertyStates().Any(x => x.IsModified);

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
            => _formState.GetStrayValidationMessages();

        internal PropertyState? GetPropertyState(IPropertyMetadata property, bool createWhenNotFound = true)
            => _formState.GetPropertyState(property, createWhenNotFound);

        internal PropertyState? GetPropertyState(string propertyName)
            => _formState.GetPropertyState(propertyName);

        private void ValidateModel()
        {
            _formState.ValidateModel();

            OnValidationStateChanged?.Invoke(this, new ValidationStateChangedEventArgs(isValid: !_formState.GetValidationMessages().Any()));
        }

        private void ValidateProperty(IPropertyMetadata property)
        {
            _formState.ValidateProperty(property);

            OnValidationStateChanged?.Invoke(this, new ValidationStateChangedEventArgs(isValid: !_formState.GetValidationMessages().Any()));
        }
    }
}
