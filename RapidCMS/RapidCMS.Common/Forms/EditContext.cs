using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using RapidCMS.Common.Data;
using RapidCMS.Common.Enums;
using RapidCMS.Common.Extensions;
using RapidCMS.Common.Models.Metadata;

namespace RapidCMS.Common.Forms
{
    // TODO: fix memory leak due to events
    // TODO: make EditContext expose serviceProvider via interface
    // TODO: remove DataContext from EditContext
    public sealed class EditContext : IServiceProvider
    {
        private readonly Dictionary<IPropertyMetadata, PropertyState> _fieldStates = new Dictionary<IPropertyMetadata, PropertyState>();
        private readonly IServiceProvider _serviceProvider;

        internal EditContext(IEntity entity, UsageType usageType, IServiceProvider serviceProvider)
        {
            Entity = entity ?? throw new ArgumentNullException(nameof(entity));
            UsageType = usageType;
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public IEntity Entity { get; private set; }
        public UsageType UsageType { get; private set; }

        internal void SwapEntity(IEntity entity)
        {
            Entity = entity;
        }

        // TODO: this thing here is weird
        internal DataContext DataContext { get; set; }

        public event EventHandler<FieldChangedEventArgs> OnFieldChanged;

        public event EventHandler<ValidationStateChangedEventArgs> OnValidationStateChanged;

        public void NotifyPropertyStartedListening(IPropertyMetadata property)
        {
            GetPropertyState(property);
        }

        public void NotifyPropertyChanged(IPropertyMetadata property)
        {
            ValidateProperty(property);

            GetPropertyState(property).IsModified = true;
            OnFieldChanged?.Invoke(this, new FieldChangedEventArgs(property));
        }

        public void NotifyPropertyBusy(IPropertyMetadata property)
        {
            GetPropertyState(property).IsBusy = true;
            OnValidationStateChanged?.Invoke(this, new ValidationStateChangedEventArgs(false));
        }

        public void NotifyPropertyFinished(IPropertyMetadata property)
        {
            GetPropertyState(property).IsBusy = false;
            OnValidationStateChanged?.Invoke(this, new ValidationStateChangedEventArgs());
        }

        public bool IsValid()
        {
            ValidateModel();

            return !HasValidationMessages();
        }

        public bool IsModified()
        {
            return _fieldStates.Any(x => x.Value.IsModified);
        }

        public bool IsValid(IPropertyMetadata property)
        {
            return !GetPropertyState(property).GetValidationMessages().Any();
        }

        public bool WasValidated(IPropertyMetadata property)
        {
            return GetPropertyState(property).WasValidated;
        }

        public IEnumerable<string> GetValidationMessages()
        {
            foreach (var state in _fieldStates)
            {
                foreach (var message in state.Value.GetValidationMessages())
                {
                    yield return message;
                }
            }
        }

        public IEnumerable<string> GetValidationMessages(IPropertyMetadata property)
        {
            if (_fieldStates.TryGetValue(property, out var state))
            {
                foreach (var message in state.GetValidationMessages())
                {
                    yield return message;
                }
            }
        }

        public void AddValidationMessage(IPropertyMetadata property, string message)
        {
            GetPropertyState(property).AddMessage(message);
            GetPropertyState(property).WasValidated = true;
        }

        private bool HasValidationMessages()
        {
            return GetValidationMessages().Any();
        }

        private PropertyState GetPropertyState(IPropertyMetadata property)
        {
            if (!_fieldStates.TryGetValue(property, out var fieldState))
            {
                fieldState = new PropertyState(property);
                _fieldStates.Add(property, fieldState);
            }

            return fieldState;
        }

        private PropertyState? GetFieldState(string propertyName)
        {
            return _fieldStates.SingleOrDefault(field => field.Key.PropertyName == propertyName).Value;
        }

        private void ClearAllFieldStates()
        {
            foreach (var fieldState in _fieldStates)
            {
                fieldState.Value.ClearMessages();
            }
        }

        private void ValidateModel()
        {
            var context = new ValidationContext(Entity, _serviceProvider, null);
            var results = new List<ValidationResult>();

            try
            {
                // even though this says Try, and therefore it should not throw an error, IT DOES when a given property is not part of Entity
                Validator.TryValidateObject(Entity, context, results, true);
            }
            catch
            {

            }

            ClearAllFieldStates();

            _fieldStates
                .Where(kv => kv.Value.IsBusy)
                .ForEach(kv => results.Add(new ValidationResult(
                    $"The {kv.Key.PropertyName} field indicates it is performing an asynchronous task which must be awaited.",
                    new[] { kv.Key.PropertyName })));

            if (DataContext != null)
            {
                results.AddRange(DataContext.ValidateRelations(Entity));
            }

            foreach (var result in results)
            {
                if (!result.MemberNames.Any())
                {
                    throw new InvalidOperationException("Only validators which explicitly specify which member is invalid should be used.");
                }

                result.MemberNames.ForEach(name => GetFieldState(name)?.AddMessage(result.ErrorMessage));
            }

            _fieldStates.ForEach(kv => kv.Value.WasValidated = true);

            OnValidationStateChanged?.Invoke(this, new ValidationStateChangedEventArgs(isValid: !HasValidationMessages()));
        }

        private void ValidateProperty(IPropertyMetadata property)
        {
            var context = new ValidationContext(Entity, _serviceProvider, null)
            {
                MemberName = property.PropertyName
            };
            var results = new List<ValidationResult>();

            try
            {
                // even though this says Try, and therefore it should not throw an error, IT DOES when a given property is not part of Entity
                Validator.TryValidateProperty(property.Getter(Entity), context, results);
            }
            catch
            {

            }

            results.AddRange(DataContext.ValidateRelation(Entity, property));

            var state = GetPropertyState(property);
            state.ClearMessages();
            state.WasValidated = true;

            foreach (var result in results)
            {
                state.AddMessage(result.ErrorMessage);
            }

            OnValidationStateChanged?.Invoke(this, new ValidationStateChangedEventArgs(isValid: !HasValidationMessages()));
        }

        public object GetService(Type serviceType)
        {
            return _serviceProvider.GetService(serviceType);
        }
    }
}
