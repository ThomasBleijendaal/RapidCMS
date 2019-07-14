using System.Collections.Generic;
using System.Linq;
using RapidCMS.Common.Models;

namespace RapidCMS.UI.Containers
{
    public abstract class CustomContainer
    {
        protected Dictionary<string, CustomTypeRegistration>? _customRegistrations;

        public CustomContainer(IEnumerable<CustomTypeRegistration>? registrations)
        {
            if (registrations != null)
            {
                _customRegistrations = registrations.ToDictionary(x => x.Alias);
            }
        }
    }
}
