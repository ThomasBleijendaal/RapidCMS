using System.Collections.Generic;
using System.Linq;
using RapidCMS.Common.Models;
using RapidCMS.Common.Models.Config;

namespace RapidCMS.Common.Providers
{
    internal class CustomRegistrationProvider : ICustomRegistrationProvider
    {
        public CustomRegistrationProvider(CmsConfig cmsConfig)
        {
            CustomDashboardSectionRegistrations = cmsConfig.CustomDashboardSectionRegistrations.ToList();
            CustomLoginRegistration = cmsConfig.CustomLoginRegistration;
        }

        private List<CustomTypeRegistration> CustomDashboardSectionRegistrations { get; set; }
        private CustomTypeRegistration? CustomLoginRegistration { get; set; }

        IEnumerable<CustomTypeRegistration> ICustomRegistrationProvider.CustomDashboardSectionRegistrations => CustomDashboardSectionRegistrations;
        CustomTypeRegistration? ICustomRegistrationProvider.CustomLoginRegistration => CustomLoginRegistration;
    }
}
