using Microsoft.AspNetCore.Components;
using RapidCMS.Common.Models.UI;
using RapidCMS.UI.Containers;

namespace RapidCMS.UI.Components.Sections
{
    public class BaseSection : ComponentBase
    {
        [Parameter] public SectionUI? Section { get; private set; }

        [CascadingParameter(Name = "CustomSections")] protected CustomSectionRenderFragmentContainer CustomSections { get; set; }
    }
}
