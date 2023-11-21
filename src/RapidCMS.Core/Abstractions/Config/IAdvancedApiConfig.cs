namespace RapidCMS.Core.Abstractions.Config;

public interface IAdvancedApiConfig
{
    /// <summary>
    /// Setting this true prevents DataAnnotationEntityValidator from being added to all repositories automatically.
    /// </summary>
    bool RemoveDataAnnotationEntityValidator { get; set; }
}
