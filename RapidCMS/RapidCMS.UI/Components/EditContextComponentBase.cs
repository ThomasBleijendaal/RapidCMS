using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using RapidCMS.Common.Validation;

namespace RapidCMS.UI.Components
{
    public abstract class EditContextComponentBase : ComponentBase, IDisposable
    {
        [CascadingParameter(Name = "EditContext")] private EditContext CascadedEditContext { get; set; }
        protected EditContext EditContext { get; set; }

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

        protected abstract void AttachValidationStateChangedListener();
        protected abstract void DetachValidationStateChangedListener();

        public virtual void Dispose()
        {
            DetachValidationStateChangedListener();
        }
    }
}
