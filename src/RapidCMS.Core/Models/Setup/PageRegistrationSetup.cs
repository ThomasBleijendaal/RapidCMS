using System.Collections.Generic;
using System.Linq;
using RapidCMS.Core.Abstractions.Config;
using RapidCMS.Core.Abstractions.Setup;
using RapidCMS.Core.Extensions;

namespace RapidCMS.Core.Models.Setup
{
    internal class PageRegistrationSetup : IPageSetup
    {
        public PageRegistrationSetup(IPageConfig config)
        {
            Name = config.Name;
            Alias = config.Alias;
            Icon = config.Icon;
            Sections = config.SectionRegistrations.Select(x => new CustomTypeRegistrationSetup(x) as ITypeRegistration).ToList();
        }

        public string Name { get; private set; }
        public string Alias { get; private set; }
        public string Icon { get; private set; }
        public List<ITypeRegistration> Sections { get; private set; }
    }
}
