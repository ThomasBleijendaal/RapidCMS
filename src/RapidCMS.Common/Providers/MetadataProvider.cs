using RapidCMS.Common.Models.Config;

namespace RapidCMS.Common.Providers
{
    internal class MetadataProvider : IMetadataProvider
    {
        public MetadataProvider(CmsConfig cmsConfig)
        {
            SiteName = cmsConfig.SiteName;
            IsDevelopment = cmsConfig.IsDevelopment;
        }

        public string SiteName { get; private set; }
        public bool IsDevelopment { get; private set; }
    }
}
