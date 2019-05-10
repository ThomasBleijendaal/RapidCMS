using Microsoft.AspNetCore.Components;
using RapidCMS.Common.Models.UI;

#nullable enable

namespace RapidCMS.UI.Components.Sections
{
    public class BaseSection : ComponentBase
    {
        [Parameter]
        public SectionUI? Section { get; private set; }

        [Parameter]
        public UISubject? Subject { get; private set; }
    }
}
