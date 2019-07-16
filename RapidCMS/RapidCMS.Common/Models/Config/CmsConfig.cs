using System;
using System.Collections.Generic;
using System.Linq;

namespace RapidCMS.Common.Models.Config
{
    // TODO: validate incoming parameters

    public class CmsConfig : ICollectionRoot
    {
        internal string SiteName { get; set; } = "RapidCMS";
        internal bool AllowAnonymousUsage { get; set; } = false;

        public List<CollectionConfig> Collections { get; set; } = new List<CollectionConfig>();
        internal List<CustomTypeRegistration> CustomButtonRegistrations { get; set; } = new List<CustomTypeRegistration>();
        internal List<CustomTypeRegistration> CustomEditorRegistrations { get; set; } = new List<CustomTypeRegistration>();
        internal List<CustomTypeRegistration> CustomSectionRegistrations { get; set; } = new List<CustomTypeRegistration>();
        internal List<CustomTypeRegistration> CustomDashboardSectionRegistrations { get; set; } = new List<CustomTypeRegistration>();
        internal CustomTypeRegistration? CustomLoginRegistration { get; set; }

        public CmsConfig AddCustomButton(Type buttonType)
        {
            CustomButtonRegistrations.Add(new CustomTypeRegistration(buttonType));

            return this;
        }

        public CmsConfig AddCustomEditor(Type editorType)
        {
            CustomEditorRegistrations.Add(new CustomTypeRegistration(editorType));

            return this;
        }

        public CmsConfig AddCustomSection(Type sectionType)
        {
            CustomSectionRegistrations.Add(new CustomTypeRegistration(sectionType));

            return this;
        }

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
