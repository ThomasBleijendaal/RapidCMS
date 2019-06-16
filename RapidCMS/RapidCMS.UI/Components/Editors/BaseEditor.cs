using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using RapidCMS.Common.Data;
using RapidCMS.Common.Enums;
using RapidCMS.Common.Models.Metadata;
using RapidCMS.Common.Validation;
using RapidCMS.Common.ValueMappers;

namespace RapidCMS.UI.Components.Editors
{
    public class BaseEditor : EditContextComponentBase
    {
        protected ValidationState State { get; private set; }

        [Parameter] protected IEntity Entity { get; private set; }

        [Parameter] protected IPropertyMetadata Property { get; private set; }

        [Parameter] protected IValueMapper ValueMapper { get; private set; }

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

        protected override void AttachValidationStateChangedListener()
        {
            if (EditContext != null)
            {
                EditContext.OnValidationStateChanged += ValidationStateChangeHandler;
                EditContext.NotifyPropertyStartedListening(Property);
            }
        }

        protected override void DetachValidationStateChangedListener()
        {
            if (EditContext != null)
            {
                EditContext.OnValidationStateChanged -= ValidationStateChangeHandler;
            }
        }
    }
}
