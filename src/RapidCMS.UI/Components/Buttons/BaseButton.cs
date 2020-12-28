using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Forms;
using RapidCMS.UI.Models;

namespace RapidCMS.UI.Components.Buttons
{
    public class BaseButton : EditContextComponentBase
    {
        [Inject] private IJSRuntime JsRuntime { get; set; } = default!;

        [Inject] private ILanguageResolver LanguageResolver { get; set; } = default!;

        [Parameter] public ButtonViewModel Model { get; set; } = default!;

        protected bool FormIsValid { get; private set; }

        protected async Task ButtonClickAsync(object? customData = null)
        {
            if (Model.RequiresValidForm && !(EditContext?.IsValid() ?? true))
            {
                return;
            }

            if (!Model.ShouldConfirm || await JsRuntime.InvokeAsync<bool>("confirm", LanguageResolver.ResolveText("Are you sure?")))
            {
                Model.NotifyClick(EditContext!, customData);
            }
        }

        private void ValidationStateChangeHandler(object? sender, ValidationStateChangedEventArgs e)
        {
            FormIsValid = e.IsValid != false;

            StateHasChanged();
        }

        protected override void AttachValidationStateChangedListener()
        {
            if (EditContext != null)
            {
                EditContext.OnValidationStateChanged += ValidationStateChangeHandler;
                FormIsValid = true;
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
