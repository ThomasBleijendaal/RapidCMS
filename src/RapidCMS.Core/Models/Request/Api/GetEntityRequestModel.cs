using RapidCMS.Core.Enums;

namespace RapidCMS.Core.Models.Request.Api;

public class GetEntityRequestModel
{
    public UsageType UsageType { get; set; }
    public EntityDescriptor Subject { get; set; } = default!;
}
