using System.Collections.Generic;
using System.Linq;
using RapidCMS.Core.Extensions;
using RapidCMS.Core.Models.Setup;

namespace RapidCMS.Core.Extensions;

internal static class ButtonSetupExtensions
{
    public static IEnumerable<ButtonSetup> GetAllButtons(this IEnumerable<ButtonSetup> buttons)
    {
        return buttons.SelectMany(x => x.Buttons.Any() ? x.Buttons.AsEnumerable() : new[] { x }).ToList();
    }
}
