using Microsoft.AspNetCore.Components;
using RapidCMS.Common.Models.UI;

namespace RapidCMS.UI.Components.Sections
{
    public class BaseEditContextSection : EditContextComponentBase
    {
        [Parameter] public SectionUI? Section { get; set; }

        protected override void AttachValidationStateChangedListener()
        {
            
        }

        protected override void DetachValidationStateChangedListener()
        {
            
        }
    }
}
