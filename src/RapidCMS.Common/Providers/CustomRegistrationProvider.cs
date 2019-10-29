using System.Collections.Generic;
using System.Linq;
using RapidCMS.Common.Models;
using RapidCMS.Common.Models.Config;

namespace RapidCMS.Common.Providers
{
    internal class CustomRegistrationProvider : ICustomRegistrationProvider
    {
        private readonly CmsConfig _cmsConfig;

        public CustomRegistrationProvider(CmsConfig cmsConfig)
        {
            _cmsConfig = cmsConfig;
        }

        IEnumerable<CustomTypeRegistration> ICustomRegistrationProvider.CustomDashboardSectionRegistrations => _cmsConfig.CustomDashboardSectionRegistrations.ToList();
        CustomTypeRegistration? ICustomRegistrationProvider.CustomLoginScreenRegistration => _cmsConfig.CustomLoginScreenRegistration;
        CustomTypeRegistration? ICustomRegistrationProvider.CustomLoginStatusRegistration => _cmsConfig.CustomLoginStatusRegistration;
    }
}
