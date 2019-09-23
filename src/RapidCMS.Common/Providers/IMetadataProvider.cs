namespace RapidCMS.Common.Providers
{
    public interface IMetadataProvider
    {
        string SiteName { get; }
        bool IsDevelopment { get; }
    }
}
