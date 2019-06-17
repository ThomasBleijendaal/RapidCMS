using Microsoft.AspNetCore.Components;
using RapidCMS.Common.Models.UI;

namespace RapidCMS.UI.Components.Sections
{
    public class BaseSection : EditContextComponentBase
    {
        [Parameter]
        public SectionUI? Section { get; private set; }

        protected override void AttachValidationStateChangedListener()
        {
            
        }

        protected override void DetachValidationStateChangedListener()
        {
            
        }
    }
}
