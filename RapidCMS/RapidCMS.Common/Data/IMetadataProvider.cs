namespace RapidCMS.Common.Data
{
    public interface IMetadataProvider
    {
        string SiteName { get; }
        bool IsDevelopment { get; }
    }
}
