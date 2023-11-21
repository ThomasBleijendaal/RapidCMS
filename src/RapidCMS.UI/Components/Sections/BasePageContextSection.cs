using System;
using Microsoft.AspNetCore.Components;
using RapidCMS.Core.Forms;
using RapidCMS.Core.Models.UI;

namespace RapidCMS.UI.Components.Sections;

/// <summary>
/// Use this section on pages nested in collections.
/// </summary>
public class BasePageContextSection : ComponentBase
{
    [Parameter] public SectionUI? Section { get; set; }

    [CascadingParameter(Name = "PageContext")] public PageContext PageContext { get; set; } = default!;

    protected Guid SectionId { get; } = Guid.NewGuid();
}
