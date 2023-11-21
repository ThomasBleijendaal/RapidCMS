namespace RapidCMS.Core.Models.Request.Api;

public class PersistRelatedEntityRequestModel
{
    public EntityDescriptor Subject { get; set; } = default!;
    public EntityDescriptor Related { get; set; } = default!;

    public Actions Action { get; set; }

    public enum Actions
    {
        Add,
        Remove
    }
}
