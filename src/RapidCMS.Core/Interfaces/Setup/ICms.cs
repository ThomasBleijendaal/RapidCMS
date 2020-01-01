namespace RapidCMS.Core.Interfaces.Setup
{
    internal interface ICms
    {
        string SiteName { get; }
        bool IsDevelopment { get; }
        bool AllowAnonymousUsage { get; }

        int SemaphoreMaxCount { get; }
    }
}
