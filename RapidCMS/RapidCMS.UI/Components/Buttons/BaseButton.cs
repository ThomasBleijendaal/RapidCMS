using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using RapidCMS.Common.Validation;
using System;
using System.Threading.Tasks;


namespace RapidCMS.UI.Components.Buttons
{
    public class BaseButton<TContext> : ComponentBase, IDisposable
    {
        [Inject]
        private IJSRuntime JsRuntime { get; set; }

        [CascadingParameter]
        protected ButtonContext<TContext> Context { get; set; }

        protected bool FormIsValid { get; private set; } = true;

        protected async Task ButtonClickAsync(object? customData = null)
        {
            var confirm = !Context.ShouldConfirm || await JsRuntime.InvokeAsync<bool>("confirm", "Are you sure?");

            if (confirm)
            {
                await Context.CallbackAsync.Invoke(Context.ButtonId, Context.Context, customData);
            }
        }

        protected override Task OnParametersSetAsync()
        {
            Context.EditContext.OnValidationStateChanged += OnValidationStateChanged;

            return base.OnParametersSetAsync();
        }

        private void OnValidationStateChanged(object sender, ValidationStateChangedEventArgs e)
        {
            FormIsValid = e.IsValid;

            StateHasChanged();
        }

        void IDisposable.Dispose()
        {
            Context.EditContext.OnValidationStateChanged -= OnValidationStateChanged;
        }
    }
}
