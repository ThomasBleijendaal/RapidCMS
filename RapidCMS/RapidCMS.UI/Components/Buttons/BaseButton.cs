using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Threading.Tasks;

namespace RapidCMS.UI.Components.Buttons
{
    public class BaseButton<TContext> : ComponentBase
    {
        [Inject]
        private IJSRuntime JsRuntime { get; set; }

        [CascadingParameter]
        protected ButtonContext<TContext> Context { get; set; }

        protected async Task ButtonClickAsync()
        {
            var confirm = !Context.ShouldConfirm || await JsRuntime.InvokeAsync<bool>("confirm", "Are you sure?");

            if (confirm)
            {
                await Context.CallbackAsync.Invoke(Context.ButtonId, Context.Context);
            }
        }
    }
}
