using Microsoft.AspNetCore.Components;
using RapidCMS.Core.Models.UI;

namespace RapidCMS.UI.Components.Sections
{
    public class BaseEditContextSection : EditContextComponentBase
    {
        [Parameter] public SectionUI? Section { get; set; }

        protected override void OnInitialized()
        {
            EditContext.OnFieldChanged += EditContext_OnFieldChanged;
        }

        private void EditContext_OnFieldChanged(object? sender, Core.Forms.FieldChangedEventArgs e)
        {
            StateHasChanged();
        }

        protected override void AttachValidationStateChangedListener()
        {
            
        }

        protected override void DetachValidationStateChangedListener()
        {
            
        }

        public override void Dispose()
        {
            EditContext.OnFieldChanged -= EditContext_OnFieldChanged;
            base.Dispose();
        }
    }
}
