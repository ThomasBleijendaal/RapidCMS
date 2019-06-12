using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RapidCMS.Common.Data;
using RapidCMS.Common.Extensions;
using RapidCMS.Common.Models.Metadata;
using RapidCMS.Common.Services;

namespace RapidCMS.Common.Validation
{
    // TODO: change field to property
    // TOOD: change IFullPropertyMetadata to IPropertyMetadata
    public class EditContext
    {
        private readonly Dictionary<IFullPropertyMetadata, FieldState> _fieldStates = new Dictionary<IFullPropertyMetadata, FieldState>();
        private readonly IServiceProvider _serviceProvider;

        public EditContext(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IEntity Entity { get; set; }

        public event EventHandler<FieldChangedEventArgs> OnFieldChanged;

        public event EventHandler<ValidationStateChangedEventArgs> OnValidationStateChanged;

        public void NotifyFieldChanged(IFullPropertyMetadata property)
        {
            ValidateProperty(property);

            GetFieldState(property).IsModified = true;
            OnFieldChanged?.Invoke(this, new FieldChangedEventArgs(property));
        }

        public bool IsValid()
        {
            ValidateModel();

            return HasValidationMessages();
        }

        public bool IsModified()
        {
            return _fieldStates.Any(x => x.Value.IsModified);
        }

        public bool IsValid(IFullPropertyMetadata property)
        {
            return !GetFieldState(property).GetValidationMessages().Any();
        }

        public bool WasValidated(IFullPropertyMetadata property)
        {
            return GetFieldState(property).WasValidated;
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

        public IEnumerable<string> GetValidationMessages(IFullPropertyMetadata property)
        {
            if (_fieldStates.TryGetValue(property, out var state))
            {
                foreach (var message in state.GetValidationMessages())
                {
                    yield return message;
                }
            }
        }

        private bool HasValidationMessages()
        {
            return GetValidationMessages().Any();
        }

        private FieldState GetFieldState(IFullPropertyMetadata property)
        {
            if (!_fieldStates.TryGetValue(property, out var fieldState))
            {
                fieldState = new FieldState(property);
                _fieldStates.Add(property, fieldState);
            }

            return fieldState;
        }

        private FieldState? GetFieldState(string propertyName)
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

            Validator.TryValidateObject(Entity, context, results, true);

            ClearAllFieldStates();

            _fieldStates.ForEach(kv => kv.Value.WasValidated = true);

            foreach (var result in results)
            {
                result.MemberNames.ForEach(name => GetFieldState(name)?.AddMessage(result.ErrorMessage));
            }

            OnValidationStateChanged?.Invoke(this, new ValidationStateChangedEventArgs(isValid: !HasValidationMessages()));
        }

        private void ValidateProperty(IFullPropertyMetadata property)
        {
            var context = new ValidationContext(Entity, _serviceProvider, null)
            {
                MemberName = property.PropertyName
            };
            var results = new List<ValidationResult>();

            Validator.TryValidateProperty(property.Getter(Entity), context, results);

            var state = GetFieldState(property);
            state.ClearMessages();
            state.WasValidated = true;

            foreach (var result in results)
            {
                state.AddMessage(result.ErrorMessage);
            }

            OnValidationStateChanged?.Invoke(this, new ValidationStateChangedEventArgs(isValid: !HasValidationMessages()));
        }
    }

    internal class FieldState
    {
        private List<string> _messages = new List<string>();

        private IFullPropertyMetadata _property;

        public FieldState(IFullPropertyMetadata property)
        {
            _property = property ?? throw new ArgumentNullException(nameof(property));
        }

        public bool WasValidated { get; set; }
        public bool IsModified { get; set; }

        public IEnumerable<string> GetValidationMessages()
        {
            return _messages;
        }

        public void AddMessage(string message)
        {
            _messages.Add(message);
        }

        public void ClearMessages()
        {
            _messages.Clear();
        }
    }

    public class FieldChangedEventArgs
    {
        public FieldChangedEventArgs(IFullPropertyMetadata fullPropertyMetadata)
        {
            FullPropertyMetadata = fullPropertyMetadata;
        }

        public IFullPropertyMetadata FullPropertyMetadata { get; }
    }
    
    public class ValidationStateChangedEventArgs
    {
        public ValidationStateChangedEventArgs(bool isValid)
        {
            IsValid = isValid;
        }

        public bool IsValid { get; private set; }
    }
}
