namespace RapidCMS.Core.Abstractions.Config
{
    public interface IAdvancedCmsConfig
    {
        /// <summary>
        /// This count controls the amount of concurrent repository calls that can be
        /// performed at the same time. For some scenarios (like server-side blazor with EF without MARS), 
        /// this number should be strictly 1, but can be higher for other scenarios. 
        /// </summary>
        int SemaphoreCount { get; set; }

        /// <summary>
        /// Setting this true prevents DataAnnotationEntityValidator from being added to all collections automatically.
        /// </summary>
        bool RemoveDataAnnotationEntityValidator { get; set; }
    }
}
