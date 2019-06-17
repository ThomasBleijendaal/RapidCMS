using Microsoft.AspNetCore.Components;
using RapidCMS.Common.Models.UI;
using RapidCMS.Common.Validation;

namespace RapidCMS.UI.Components.Sections
{
    // TODO: EditContextComponentBase?
    public class BaseSection : ComponentBase
    {
        [Parameter]
        public SectionUI? Section { get; private set; }

        [CascadingParameter(Name = "EditContext")] public EditContext EditContext { get; private set; }
    }
}
