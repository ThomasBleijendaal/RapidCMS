using System;
using System.Collections.Generic;
using System.Text;
using RapidCMS.Common.Models.Metadata;

namespace RapidCMS.Common.Validation
{
    public class EditContext
    {
        private readonly Dictionary<string, FieldState> _fieldStates = new Dictionary<string, FieldState>();

        public event EventHandler<FieldChangedEventArgs> OnFieldChanged;

        public event EventHandler<ValidationRequestedEventArgs> OnValidationRequested;

        public event EventHandler<ValidationStateChangedEventArgs> OnValidationStateChanged;

        public bool IsValid { get; set; }

        public void NotifyFieldChanged(IFullPropertyMetadata fullPropertyMetadata)
        {
            GetFieldState(fullPropertyMetadata).IsModified = true;
            OnFieldChanged?.Invoke(this, new FieldChangedEventArgs(fullPropertyMetadata));

            IsValid = !IsValid;

            OnValidationStateChanged?.Invoke(this, new ValidationStateChangedEventArgs(IsValid));
        }

        public void RequestValidation()
        {

        } 

        internal FieldState GetFieldState(IFullPropertyMetadata fullPropertyMetadata)
        {
            if (!_fieldStates.TryGetValue(fullPropertyMetadata.Fingerprint, out var fieldState))
            {
                fieldState = new FieldState();
                _fieldStates.Add(fullPropertyMetadata.Fingerprint, fieldState);
            }

            return fieldState;
        }


    }

    internal class FieldState
    {
        public bool IsModified { get; set; }
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

    public class ValidationStateChangedEventArgs {
        public ValidationStateChangedEventArgs(bool isValid)
        {
            IsValid = isValid;
        }

        public bool IsValid { get; set; }
    }
}
