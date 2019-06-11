using System;
using System.Threading.Tasks;
using RapidCMS.Common.Validation;

namespace RapidCMS.UI.Components.Buttons
{
    public class ButtonContext<TContext>
    {
        public ButtonContext(EditContext editContext)
        {
            EditContext = editContext;
        }

        public string Label { get; set; }
        public string Icon { get; set; }
        public string ButtonId { get; set; }
        public Func<string, TContext, object?, Task> CallbackAsync { get; set; }
        public TContext Context { get; set; }
        public EditContext EditContext { get; }
        public bool ShouldConfirm { get; set; }
        public bool IsPrimary { get; set; }

    }
}
