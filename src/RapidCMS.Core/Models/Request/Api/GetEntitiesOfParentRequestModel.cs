using RapidCMS.Core.Models.Data;

namespace RapidCMS.Core.Models.Request.Api;

public class GetEntitiesOfParentRequestModel : GetEntitiesRequestModel
{
    public string? ParentPath { get; set; }
}
