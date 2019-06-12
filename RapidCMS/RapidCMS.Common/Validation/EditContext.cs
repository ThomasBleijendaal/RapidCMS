using System;
using System.Collections.Generic;
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

        public event EventHandler<ValidationRequestedEventArgs> OnValidationRequested;

        public event EventHandler<ValidationStateChangedEventArgs> OnValidationStateChanged;

        public void NotifyFieldChanged(IFullPropertyMetadata property)
        {
            GetFieldState(property).IsModified = true;
            OnFieldChanged?.Invoke(this, new FieldChangedEventArgs(property));

            // TODO: 
            //IsValid = !IsValid;

            //OnValidationStateChanged?.Invoke(this, new ValidationStateChangedEventArgs(IsValid));
        }

        public async Task<bool> IsValidAsync()
        {
            var validator = _serviceProvider.GetService<IValidationService>();

            var result = await validator.IsValidAsync(Entity);

            OnValidationStateChanged?.Invoke(this, new ValidationStateChangedEventArgs(result));

            return result;
        }

        public bool IsModified()
        {
            return _fieldStates.Any(x => x.Value.IsModified);
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

        private FieldState GetFieldState(IFullPropertyMetadata property)
        {
            if (!_fieldStates.TryGetValue(property, out var fieldState))
            {
                fieldState = new FieldState(property);
                _fieldStates.Add(property, fieldState);
            }

            return fieldState;
        }

        private void ValidateModel()
        {

        }

        private void ValidateProperty(IFullPropertyMetadata property)
        {

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

        public bool IsModified { get; set; }

        public IEnumerable<string> GetValidationMessages()
        {
            return _messages;
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
    public class ValidationRequestedEventArgs { }

    public class ValidationStateChangedEventArgs
    {
        public ValidationStateChangedEventArgs(bool isValid)
        {
            IsValid = isValid;
        }

        public bool IsValid { get; set; }
    }

    //public class ValidationMessageStore
    //{
    //    private readonly EditContext _editContext;
    //    private readonly Dictionary<IFullPropertyMetadata, List<string>> _messages = new Dictionary<IFullPropertyMetadata, List<string>>();

    //    public ValidationMessageStore(EditContext editContext)
    //    {
    //        _editContext = editContext ?? throw new ArgumentNullException(nameof(editContext));
    //    }

    //    public void Add(IFullPropertyMetadata property, string message) => GetOrCreateMessagesListForField(property).Add(message);

    //    public void AddRange(IFullPropertyMetadata property, IEnumerable<string> messages) => GetOrCreateMessagesListForField(property).AddRange(messages);

    //    public IEnumerable<string> this[IFullPropertyMetadata property] => _messages.TryGetValue(property, out var messages) ? messages : Enumerable.Empty<string>();

    //    public void Clear()
    //    {
    //        foreach (var fieldIdentifier in _messages.Keys)
    //        {
    //            DissociateFromField(fieldIdentifier);
    //        }

    //        _messages.Clear();
    //    }

    //    public void Clear(IFullPropertyMetadata property)
    //    {
    //        DissociateFromField(property);
    //        _messages.Remove(property);
    //    }

    //    private List<string> GetOrCreateMessagesListForField(IFullPropertyMetadata property)
    //    {
    //        if (!_messages.TryGetValue(property, out var messagesForField))
    //        {
    //            messagesForField = new List<string>();
    //            _messages.Add(property, messagesForField);
    //            AssociateWithField(property);
    //        }

    //        return messagesForField;
    //    }

    //    private void AssociateWithField(IFullPropertyMetadata property) => _editContext.GetFieldState(property).AssociateWithValidationMessageStore(this);

    //    private void DissociateFromField(IFullPropertyMetadata property) => _editContext.GetFieldState(property)?.DissociateFromValidationMessageStore(this);
    //}
}
