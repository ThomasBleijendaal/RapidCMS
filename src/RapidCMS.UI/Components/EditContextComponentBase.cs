using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using RapidCMS.Core.Forms;

namespace RapidCMS.UI.Components
{
    // TODO: implement disposablecomponent instead of componentbase
    public abstract class EditContextComponentBase : ComponentBase, IDisposable
    {
        [CascadingParameter(Name = "EditContext")] private FormEditContext CascadedEditContext { get; set; } = default!;
        protected FormEditContext EditContext { get; set; } = default!;

        public override Task SetParametersAsync(ParameterView parameters)
        {
            parameters.SetParameterProperties(this);

            if (EditContext == null)
            {
                if (CascadedEditContext == null)
                {
                    throw new InvalidOperationException($"{GetType()} requires a CascadingParameter {nameof(EditContext)}.");
                }

                EditContext = CascadedEditContext;

                AttachListener();
            }
            else if (EditContext != CascadedEditContext)
            {
                DetachListener();

                EditContext = CascadedEditContext;

                AttachListener();
            }

            return base.SetParametersAsync(ParameterView.Empty);
        }

        protected abstract void AttachListener();
        protected abstract void DetachListener();

        public virtual void Dispose()
        {
            DetachListener();
        }
    }
}
