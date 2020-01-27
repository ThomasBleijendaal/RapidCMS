using System.Collections.Generic;
using System.Linq;
using RapidCMS.Core.Abstractions.Setup;
using RapidCMS.Core.Extensions;

namespace RapidCMS.Core.Extensions
{
    internal static class ButtonSetupExtensions
    {
        public static IEnumerable<IButtonSetup> GetAllButtons(this IEnumerable<IButtonSetup> buttons)
        {
            return buttons.SelectMany(x => x.Buttons.Any() ? x.Buttons.AsEnumerable() : new[] { x }).ToList();
        }
    }
}
