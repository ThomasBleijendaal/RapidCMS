using Microsoft.AspNetCore.Components;
using RapidCMS.Core.Models.UI;
using RapidCMS.UI.Components.Sections;

namespace RapidCMS.UI.Extensions;

public static class SectionUIExtensions
{
    public static RenderFragment? ToRenderFragment(this SectionUI section)
    {
        if (section.CustomType == null)

        {
            return null;
        }

        return builder =>
        {
            builder.OpenComponent(0, section.CustomType);

            builder.AddAttribute(1, nameof(BaseSection.Section), section);

            builder.CloseComponent();
        };
    }
}
