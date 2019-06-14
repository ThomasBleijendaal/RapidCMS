using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using RapidCMS.Common.Data;
using RapidCMS.Common.Enums;
using RapidCMS.Common.Models.Metadata;
using RapidCMS.Common.Validation;
using RapidCMS.Common.ValueMappers;

namespace RapidCMS.UI.Components.Editors
{
    public class BaseEditor : ComponentBase, IDisposable
    {
        [CascadingParameter(Name = "EditContext")]
        private EditContext CascadedEditContext { get; set; }

        protected EditContext EditContext { get; set; }

        protected ValidationState State { get; private set; }

        [Parameter]
        protected IEntity Entity { get; private set; }

        [Parameter]
        protected IPropertyMetadata Property { get; private set; }

        [Parameter]
        protected IValueMapper ValueMapper { get; private set; }

        protected override Task OnParametersSetAsync()
        {
            if (EditContext == null)
            {
                if (CascadedEditContext == null)
                {
                    throw new InvalidOperationException($"{GetType()} requires a CascadingParameter {nameof(EditContext)}.");
                }

                EditContext = CascadedEditContext;

                AttachValidationStateChangedListener();
            }
            else if (EditContext != CascadedEditContext)
            {
                DetachValidationStateChangedListener();

                EditContext = CascadedEditContext;

                AttachValidationStateChangedListener();
            }

            return base.OnParametersSetAsync();
        }

        protected object GetValue(bool useValueMapper = true)
        {
            if (useValueMapper)
            {
                return ValueMapper.MapToEditor(null, Property.Getter(Entity));
            }
            else
            {
                return Property.Getter(Entity);
            }
        }

        protected IEnumerable<string> GetValidationMessages()
        {
            return EditContext.GetValidationMessages(Property);
        }

        
        private void ValidationStateChangeHandler(object sender, ValidationStateChangedEventArgs eventArgs)
        {
            if (EditContext.WasValidated(Property))
            {
                if (EditContext.IsValid(Property))
                {
                    State = ValidationState.Valid;
                }
                else
                {
                    State = ValidationState.Invalid;
                }
            }
            else
            {
                State = ValidationState.NotValidated;
            }

            StateHasChanged();
        }

        void IDisposable.Dispose()
        {
            DetachValidationStateChangedListener();
        }

        private void AttachValidationStateChangedListener()
        {
            if (EditContext != null)
            {
                EditContext.OnValidationStateChanged += ValidationStateChangeHandler;
                EditContext.NotifyPropertyStartedListening(Property);
            }
        }

        private void DetachValidationStateChangedListener()
        {
            if (EditContext != null)
            {
                EditContext.OnValidationStateChanged -= ValidationStateChangeHandler;
            }
        }
    }
}
