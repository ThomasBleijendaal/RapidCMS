namespace RapidCMS.Core.Abstractions.Setup;

public interface ICms
{
    string SiteName { get; }
    bool IsDevelopment { get; set; }
}
