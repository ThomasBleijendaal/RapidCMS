using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace RapidCMS.UI.Components.Buttons
{
    public class BaseButton<TContext> : ComponentBase
    {
        [CascadingParameter]
        protected ButtonContext<TContext> Context { get; set; }

        protected async Task ButtonClickAsync()
        {
            await Context.CallbackAsync.Invoke(Context.ButtonId, Context.Context);
        }
    }
}
