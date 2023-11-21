using RapidCMS.Core.Abstractions.Data;

namespace RapidCMS.Core.Models.ApiBridge.Request;

public class ReorderModel
{
    public ReorderModel() { }

    public ReorderModel(string? beforeId, string id, IParent? parent)
    {
        BeforeId = beforeId;
        Subject = new EntityDescriptorModel
        {
            Id = id,
            ParentPath = parent?.GetParentPath()?.ToPathString()
        };
    }

    public string? BeforeId { get; set; }

    public EntityDescriptorModel Subject { get; set; } = default!;
}
