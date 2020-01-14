using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Metadata;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Extensions;
using RapidCMS.Core.Forms.Validation;
using RapidCMS.Core.Providers;

namespace RapidCMS.Core.Forms
{

    public sealed class EditContext
    {
        private readonly FormState _formState = new FormState();
        private readonly List<PropertyState> _fieldStates = new List<PropertyState>();

        internal EditContext(string collectionAlias, IEntity entity, IParent? parent, UsageType usageType, IServiceProvider serviceProvider)
        {
            CollectionAlias = collectionAlias ?? throw new ArgumentNullException(nameof(collectionAlias));
            Entity = entity ?? throw new ArgumentNullException(nameof(entity));
            Parent = parent;
            UsageType = usageType;
            ServiceProvider = serviceProvider;
        }

        public string CollectionAlias { get; private set; }
        public IEntity Entity { get; private set; }
        public IParent? Parent { get; private set; }
        public UsageType UsageType { get; private set; }
        public EntityState EntityState => UsageType.HasFlag(UsageType.New) ? EntityState.IsNew : EntityState.IsExisting;

        public IServiceProvider ServiceProvider { get; private set; }

        // TODO: really not good
        [Obsolete]
        internal void SwapEntity(IEntity entity)
        {
            Entity = entity;
        }

        internal List<DataProvider> DataProviders = new List<DataProvider>();

        public event EventHandler<FieldChangedEventArgs>? OnFieldChanged;

        public event EventHandler<ValidationStateChangedEventArgs>? OnValidationStateChanged;

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

            return !HasValidationMessages();
        }

        public bool IsModified()
        {
            return _fieldStates.Any(x => x.IsModified);
        }

        public bool IsValid(IPropertyMetadata property)
        {
            return !GetPropertyState(property)!.GetValidationMessages().Any();
        }

        public bool WasValidated(IPropertyMetadata property)
        {
            return GetPropertyState(property)!.WasValidated;
        }

        public void AddValidationMessage(IPropertyMetadata property, string message)
        {
            GetPropertyState(property, true)!.AddMessage(message);
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
        {
            return _formState.GetValidationMessages();
        }

        private bool HasValidationMessages()
        {
            return _formState.GetValidationMessages().Any() || _fieldStates.Any(x => x.GetValidationMessages().Any());
        }

        internal PropertyState? GetPropertyState(IPropertyMetadata property, bool createWhenNotFound = true)
        {
            var fieldState = _fieldStates.SingleOrDefault(x => x.Property.Fingerprint == property.Fingerprint);
            if (fieldState == null)
            {
                if (!createWhenNotFound)
                {
                    return default;
                }

                fieldState = new PropertyState(property);
                _fieldStates.Add(fieldState);
            }

            return fieldState;
        }

        internal PropertyState? GetPropertyState(string propertyName)
        {
            return _fieldStates.SingleOrDefault(field => field.Property.PropertyName == propertyName);
        }

        private void ClearAllFieldStates()
        {
            _formState.ClearMessages();

            foreach (var fieldState in _fieldStates)
            {
                fieldState.ClearMessages();
            }
        }

        private void ValidateModel()
        {
            var results = GetValidationResultsForModel();

            foreach (var result in results)
            {
                var strayError = true;

                result.MemberNames.ForEach(name =>
                {
                    GetPropertyState(name)?.AddMessage(result.ErrorMessage);
                    strayError = false;
                });

                if (strayError)
                {
                    _formState.AddMessage(result.ErrorMessage);
                }
            }

            _fieldStates.ForEach(x => x.WasValidated = true);

            OnValidationStateChanged?.Invoke(this, new ValidationStateChangedEventArgs(isValid: !HasValidationMessages()));
        }

        private void ValidateProperty(IPropertyMetadata property)
        {
            IEnumerable<ValidationResult> results;

            // when a property must be validated which is not part of the entity (like a nested object)
            // validate the complete model, but only keep the results of that property
            if (Entity.GetType().GetProperty(property.PropertyName) == null)
            {
                results = GetValidationResultsForModel().Where(x => x.MemberNames.Contains(property.PropertyName));
            }
            else
            {
                results = GetValidationResultsForProperty(property);
            }

            var state = GetPropertyState(property)!;
            state.ClearMessages();
            state.WasValidated = true;

            foreach (var result in results)
            {
                state.AddMessage(result.ErrorMessage);
            }

            OnValidationStateChanged?.Invoke(this, new ValidationStateChangedEventArgs(isValid: !HasValidationMessages()));
        }

        private IEnumerable<ValidationResult> GetValidationResultsForProperty(IPropertyMetadata property)
        {
            var results = new List<ValidationResult>();
            var context = new ValidationContext(Entity, ServiceProvider, null)
            {
                MemberName = property.PropertyName
            };

            try
            {
                // even though this says Try, and therefore it should not throw an error, IT DOES when a given property is not part of Entity
                Validator.TryValidateProperty(property.Getter(Entity), context, results);
            }
            catch { }

            // TODO
            //foreach (var result in DataProviders.Where(p => p.Property == property).SelectMany(p => p.Validate(Entity, _serviceProvider)))
            //{
            //    results.Add(result);
            //}

            return results;
        }

        private IEnumerable<ValidationResult> GetValidationResultsForModel()
        {
            var context = new ValidationContext(Entity, ServiceProvider, null);
            var results = new List<ValidationResult>();

            try
            {
                // even though this says Try, and therefore it should not throw an error, IT DOES when a given property is not part of Entity
                Validator.TryValidateObject(Entity, context, results, true);
            }
            catch { }

            ClearAllFieldStates();

            _fieldStates
                .Where(kv => kv.IsBusy)
                .ForEach(kv => results.Add(new ValidationResult(
                    $"The {kv.Property.PropertyName} field indicates it is performing an asynchronous task which must be awaited.",
                    new[] { kv.Property.PropertyName })));

            // TODO
            //foreach (var result in DataProviders.SelectMany(p => p.Validate(Entity, _serviceProvider)))
            //{
            //    results.Add(result);
            //}

            return FlattenCompositeValidationResults(results);
        }

        private IEnumerable<ValidationResult> FlattenCompositeValidationResults(IEnumerable<ValidationResult> results, string? memberNamePrefix = null)
        {
            foreach (var result in results)
            {
                if (result is CompositeValidationResult composite)
                {
                    foreach (var nestedResult in FlattenCompositeValidationResults(composite.Results, composite.MemberName))
                    {
                        yield return nestedResult;
                    }
                }
                else if (string.IsNullOrWhiteSpace(memberNamePrefix))
                {
                    yield return result;
                }
                else
                {
                    yield return new ValidationResult(result.ErrorMessage, result.MemberNames.Select(x => $"{memberNamePrefix}{x}"));
                }
            }
        }
    }
}
