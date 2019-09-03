using Microsoft.AspNetCore.Components;
using RapidCMS.Common.Models.UI;

namespace RapidCMS.UI.Components.Sections
{
    public class BaseSection : ComponentBase
    {
        [Parameter] public SectionUI? Section { get; private set; }
    }
}
