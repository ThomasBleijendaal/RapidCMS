using RapidCMS.Core.Abstractions.Config;

namespace RapidCMS.Core.Models.Config.Api
{
    internal class ApiAdvancedConfig : IAdvancedApiConfig
    {
        public bool RemoveDataAnnotationEntityValidator { get; set; }
    }
}
