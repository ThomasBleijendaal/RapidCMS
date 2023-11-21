using RapidCMS.Core.Abstractions.Data;

namespace RapidCMS.Core.Models.Request.Api;

public class GetEntitiesOfRelationRequestModel : GetEntitiesRequestModel
{
    public EntityDescriptor Related { get; set; } = default!;
}
