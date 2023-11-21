namespace RapidCMS.Core.Models.Request.Api;

public class DeleteEntityRequestModel
{
    public EntityDescriptor Descriptor { get; set; } = default!;
}
