using System;
using System.Threading.Tasks;
using RapidCMS.Common.Validation;

namespace RapidCMS.UI.Components.Buttons
{
    // TODO: not really a context
    // TODO: investigate removing TContext since buttons have EditContext available
    public class ButtonContext<TContext>
    {
        public string Label { get; set; }
        public string Icon { get; set; }
        public string ButtonId { get; set; }
        public Func<string, TContext, object?, Task> CallbackAsync { get; set; }

        public TContext Context { get; set; }

        public bool ShouldConfirm { get; set; }
        public bool IsPrimary { get; set; }
        public bool RequiresValidForm { get; set; }

    }
}
