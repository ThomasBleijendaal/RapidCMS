using System.Collections.Generic;
using System.Linq;
using RapidCMS.Common.Models;

#nullable enable

namespace RapidCMS.Common.Extensions
{
    public static class ButtonExtensions
    {
        public static IEnumerable<Button> GetAllButtons(this IEnumerable<Button> buttons)
        {
            // HACK: bit of a hack
            return buttons.SelectMany(x => x.Buttons.Any() ? x.Buttons.AsEnumerable() : new[] { x }).ToList();
        }
    }
}
