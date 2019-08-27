using Microsoft.AspNetCore.Components;
using RapidCMS.Common.Models.UI;
using RapidCMS.UI.Containers;

namespace RapidCMS.UI.Components.Sections
{
    public class BaseEditContextSection : EditContextComponentBase
    {
        [Parameter] public SectionUI? Section { get; private set; }

        [CascadingParameter(Name = "Editors")] protected EditorRenderFragmentContainer Editors { get; set; }
        [CascadingParameter(Name = "CustomSections")] protected CustomSectionRenderFragmentContainer CustomSections { get; set; }

        protected override void AttachValidationStateChangedListener()
        {
            
        }

        protected override void DetachValidationStateChangedListener()
        {
            
        }
    }
}
