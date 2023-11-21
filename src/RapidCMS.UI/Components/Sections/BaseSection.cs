using System;
using Microsoft.AspNetCore.Components;
using RapidCMS.Core.Models.UI;

namespace RapidCMS.UI.Components.Sections;

/// <summary>
/// Use this section on the dashboard and in pages nested in collections that do not require access to the entity.
/// </summary>
public class BaseSection : ComponentBase
{
    [Parameter] public SectionUI? Section { get; set; }

    protected Guid SectionId { get; } = Guid.NewGuid();
}
