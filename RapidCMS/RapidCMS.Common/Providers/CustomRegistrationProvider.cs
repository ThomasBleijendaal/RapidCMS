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
            CustomButtonRegistrations = cmsConfig.CustomButtonRegistrations.ToList();
            CustomEditorRegistrations = cmsConfig.CustomEditorRegistrations.ToList();
            CustomSectionRegistrations = cmsConfig.CustomSectionRegistrations.ToList();
            CustomDashboardSectionRegistrations = cmsConfig.CustomDashboardSectionRegistrations.ToList();
            CustomLoginRegistration = cmsConfig.CustomLoginRegistration;
        }

        private List<CustomTypeRegistration> CustomButtonRegistrations { get; set; }
        private List<CustomTypeRegistration> CustomEditorRegistrations { get; set; }
        private List<CustomTypeRegistration> CustomSectionRegistrations { get; set; }
        private List<CustomTypeRegistration> CustomDashboardSectionRegistrations { get; set; }
        private CustomTypeRegistration? CustomLoginRegistration { get; set; }

        IEnumerable<CustomTypeRegistration> ICustomRegistrationProvider.CustomButtonRegistrations => CustomButtonRegistrations;
        IEnumerable<CustomTypeRegistration> ICustomRegistrationProvider.CustomEditorRegistrations => CustomEditorRegistrations;
        IEnumerable<CustomTypeRegistration> ICustomRegistrationProvider.CustomSectionRegistrations => CustomSectionRegistrations;
        IEnumerable<CustomTypeRegistration> ICustomRegistrationProvider.CustomDashboardSectionRegistrations => CustomDashboardSectionRegistrations;
        CustomTypeRegistration? ICustomRegistrationProvider.CustomLoginRegistration => CustomLoginRegistration;
    }
}
