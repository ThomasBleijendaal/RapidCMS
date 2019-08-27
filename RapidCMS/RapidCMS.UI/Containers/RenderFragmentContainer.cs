using System.Collections.Generic;
using System.Linq;
using RapidCMS.Common.Models;

namespace RapidCMS.UI.Containers
{
    public abstract class RenderFragmentContainer
    {
        protected Dictionary<string, CustomTypeRegistration>? _customRegistrations;

        public RenderFragmentContainer(IEnumerable<CustomTypeRegistration>? registrations)
        {
            if (registrations != null)
            {
                _customRegistrations = registrations.ToDictionary(x => x.Alias);
            }
        }
    }
}
