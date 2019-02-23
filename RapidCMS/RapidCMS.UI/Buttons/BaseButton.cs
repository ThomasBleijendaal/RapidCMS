using Microsoft.AspNetCore.Components;
using RapidCMS.Common.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RapidCMS.UI.Buttons
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

    public class ButtonContext<TContext>
    {
        public string Label { get; set; }
        public string Icon { get; set; }
        public string ButtonId { get; set; }
        public Func<string, TContext, Task> CallbackAsync { get; set; }
        public TContext Context { get; set; }
    }
}
