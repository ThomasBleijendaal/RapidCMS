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

        protected bool IsDisabled { get; set; }

        protected async Task ButtonClickAsync(object? customData = null)
        {
            try
            {
                IsDisabled = true;
                StateHasChanged();

                if (Model.RequiresValidForm && !(EditContext?.IsValid() ?? true))
                {
                    return;
                }

                if (!Model.ShouldConfirm || await JsRuntime.InvokeAsync<bool>("confirm", LanguageResolver.ResolveText("Are you sure?")))
                {
                    await Model.NotifyClickAsync(EditContext!, customData);
                }
            }
            finally
            {
                IsDisabled = false;
                StateHasChanged();
            }
        }

        private void ValidationStateChangeHandler(object? sender, ValidationStateChangedEventArgs e)
        {
            FormIsValid = e.IsValid != false;

            StateHasChanged();
        }

        protected override void AttachListener()
        {
            if (EditContext != null)
            {
                EditContext.OnValidationStateChanged += ValidationStateChangeHandler;
                FormIsValid = true;
            }
        }

        protected override void DetachListener()
        {
            if (EditContext != null)
            {
                EditContext.OnValidationStateChanged -= ValidationStateChangeHandler;
            }
        }
    }
}
