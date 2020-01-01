using System.Collections.Generic;
using RapidCMS.Core.Extensions;
using RapidCMS.Core.Helpers;
using RapidCMS.Core.Interfaces.Setup;
using RapidCMS.Core.Models.Config;

namespace RapidCMS.Core.Models.Setup
{
    internal class CmsSetup : ICms, ICollections, IDashboard, ILogin
    {
        internal CmsSetup(CmsConfig config)
        {
            SiteName = config.SiteName;
            IsDevelopment = config.IsDevelopment;
            AllowAnonymousUsage = config.AllowAnonymousUsage;
            SemaphoreMaxCount = config.SemaphoreMaxCount;

            Collections = ConfigProcessingHelper.ProcessCollections(config);

            CustomDashboardSectionRegistrations = config.CustomDashboardSectionRegistrations.ToList(x => new CustomTypeRegistrationSetup(x));
            if (config.CustomLoginScreenRegistration != null)
            {
                CustomLoginScreenRegistration = new CustomTypeRegistrationSetup(config.CustomLoginScreenRegistration);
            }
            if (config.CustomLoginStatusRegistration != null)
            {
                CustomLoginStatusRegistration = new CustomTypeRegistrationSetup(config.CustomLoginStatusRegistration);
            }
        }

        internal string SiteName { get; set; }
        internal bool IsDevelopment { get; set; }
        internal bool AllowAnonymousUsage { get; set; }

        internal int SemaphoreMaxCount { get; set; }

        public List<CollectionSetup> Collections { get; set; } 
        internal List<CustomTypeRegistrationSetup> CustomDashboardSectionRegistrations { get; set; } 
        internal CustomTypeRegistrationSetup? CustomLoginScreenRegistration { get; set; }
        internal CustomTypeRegistrationSetup? CustomLoginStatusRegistration { get; set; }

        string ICms.SiteName => SiteName;
        bool ICms.IsDevelopment => IsDevelopment;
        bool ICms.AllowAnonymousUsage => AllowAnonymousUsage;
        int ICms.SemaphoreMaxCount => SemaphoreMaxCount;

        IEnumerable<CollectionSetup> ICollections.Collections => Collections;

        IEnumerable<CustomTypeRegistrationSetup> IDashboard.CustomDashboardSectionRegistrations => CustomDashboardSectionRegistrations;

        CustomTypeRegistrationSetup? ILogin.CustomLoginScreenRegistration => CustomLoginScreenRegistration;
        CustomTypeRegistrationSetup? ILogin.CustomLoginStatusRegistration => CustomLoginStatusRegistration;
    }
}
