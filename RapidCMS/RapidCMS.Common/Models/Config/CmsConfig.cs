using System;
using System.Collections.Generic;
using System.Linq;

namespace RapidCMS.Common.Models.Config
{
    // TODO: validate incoming parameters

    public class CmsConfig : ICollectionRoot
    {
        internal string SiteName { get; set; } = "RapidCMS";
        internal bool IsDevelopment { get; set; }
        internal bool AllowAnonymousUsage { get; set; } = false;

        internal int SemaphoreMaxCount { get; set; } = 1;

        public List<CollectionConfig> Collections { get; set; } = new List<CollectionConfig>();
        internal List<CustomTypeRegistration> CustomDashboardSectionRegistrations { get; set; } = new List<CustomTypeRegistration>();
        internal CustomTypeRegistration? CustomLoginRegistration { get; set; }

        public CmsConfig SetCustomLogin(Type loginType)
        {
            CustomLoginRegistration = new CustomTypeRegistration(loginType);

            return this;
        }

        public CmsConfig SetSiteName(string siteName)
        {
            SiteName = siteName;

            return this;
        }

        public CmsConfig AllowAnonymousUser()
        {
            AllowAnonymousUsage = true;

            return this;
        }

        public bool IsUnique(string alias)
        {
            return !Collections.Any(col => col.Alias == alias);
        }

        public CmsConfig AddDashboardSection(Type customDashboardSectionType)
        {
            CustomDashboardSectionRegistrations.Add(new CustomTypeRegistration(customDashboardSectionType));

            return this;
        }

        /// <summary>
        /// Sets the number of concurrent IRepository actions. Will lead to deadlocks when used incorrectly.
        /// </summary>
        /// <param name="maxCount">Max number of concurrent IRepository actions.</param>
        /// <returns></returns>
        public CmsConfig DangerouslyFiddleWithSemaphoreSettings(int maxCount)
        {
            SemaphoreMaxCount = maxCount;

            return this;
        }
        
        // TODO: this should throw if collection does not implement its edit or list view
        public CmsConfig AddDashboardSection(string collectionAlias, bool edit = false)
        {
            CustomDashboardSectionRegistrations.Add(
                new CustomTypeRegistration(
                    typeof(Collection),
                    new Dictionary<string, string> {
                        { "Action", edit ? Constants.Edit : Constants.List },
                        { "CollectionAlias", collectionAlias } }));

            return this;
        }
    }
}
