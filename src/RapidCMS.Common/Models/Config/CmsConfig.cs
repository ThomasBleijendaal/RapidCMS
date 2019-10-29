using System;
using System.Collections.Generic;
using System.Linq;
using RapidCMS.Common.Extensions;
using Microsoft.AspNetCore.Components;

namespace RapidCMS.Common.Models.Config
{
    internal class CmsConfig : ICmsConfig
    {
        internal string SiteName { get; set; } = "RapidCMS";
        internal bool IsDevelopment { get; set; }
        internal bool AllowAnonymousUsage { get; set; } = false;

        internal int SemaphoreMaxCount { get; set; } = 1;

        public string Alias => "__root";

        public List<ICollectionConfig> Collections { get; set; } = new List<ICollectionConfig>();
        internal List<CustomTypeRegistration> CustomDashboardSectionRegistrations { get; set; } = new List<CustomTypeRegistration>();
        internal CustomTypeRegistration? CustomLoginScreenRegistration { get; set; }
        internal CustomTypeRegistration? CustomLoginStatusRegistration { get; set; }

        public ICmsConfig SetCustomLoginScreen(Type loginType)
        {
            if (!loginType.IsSameTypeOrDerivedFrom(typeof(ComponentBase)))
            {
                throw new InvalidOperationException($"{nameof(loginType)} must be derived of {nameof(ComponentBase)}.");
            }

            CustomLoginScreenRegistration = new CustomTypeRegistration(loginType);

            return this;
        }

        public ICmsConfig SetCustomLoginStatus(Type loginType)
        {
            if (!loginType.IsSameTypeOrDerivedFrom(typeof(ComponentBase)))
            {
                throw new InvalidOperationException($"{nameof(loginType)} must be derived of {nameof(ComponentBase)}.");
            }

            CustomLoginStatusRegistration = new CustomTypeRegistration(loginType);

            return this;
        }

        public ICmsConfig SetSiteName(string siteName)
        {
            SiteName = siteName;

            return this;
        }

        public ICmsConfig AllowAnonymousUser()
        {
            AllowAnonymousUsage = true;

            return this;
        }

        bool ICollectionConfig.IsUnique(string alias)
        {
            return !Collections.Any(col => col.Alias == alias);
        }

        public ICmsConfig AddDashboardSection(Type customDashboardSectionType)
        {
            if (!customDashboardSectionType.IsSameTypeOrDerivedFrom(typeof(ComponentBase)))
            {
                throw new InvalidOperationException($"{nameof(customDashboardSectionType)} must be derived of {nameof(ComponentBase)}.");
            }

            CustomDashboardSectionRegistrations.Add(new CustomTypeRegistration(customDashboardSectionType));

            return this;
        }

        public ICmsConfig AddDashboardSection(string collectionAlias, bool edit = false)
        {
            CustomDashboardSectionRegistrations.Add(
                new CustomTypeRegistration(
                    typeof(Collection),
                    new Dictionary<string, string> {
                        { "Action", edit ? Constants.Edit : Constants.List },
                        { "CollectionAlias", collectionAlias } }));

            return this;
        }

        public ICmsConfig DangerouslyFiddleWithSemaphoreSettings(int maxCount)
        {
            SemaphoreMaxCount = maxCount;

            return this;
        }
    }
}
