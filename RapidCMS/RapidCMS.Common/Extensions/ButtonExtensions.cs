using System.Collections.Generic;
using System.Linq;
using RapidCMS.Common.Models;

namespace RapidCMS.Common.Extensions
{
    internal static class ButtonExtensions
    {
        public static IEnumerable<Button> GetAllButtons(this IEnumerable<Button> buttons)
        {
            return buttons.SelectMany(x => x.Buttons.Any() ? x.Buttons.AsEnumerable() : new[] { x }).ToList();
        }
    }
}
