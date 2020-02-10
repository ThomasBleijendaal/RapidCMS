using RapidCMS.Core.Abstractions.Config;

namespace RapidCMS.Core.Models.Config
{
    internal class CmsAdvancedConfig : IAdvancedCmsConfig
    {
        public int SemaphoreCount { get; set; }
    }
}
