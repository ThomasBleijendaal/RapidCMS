using RapidCMS.Core.Abstractions.Data;

namespace RapidCMS.Core.Models.ApiBridge.Request;

public class DeleteModel
{
    public DeleteModel() { }

    public DeleteModel(IParent? parent)
    {
        ParentPath = parent?.GetParentPath()?.ToPathString();
    }

    public string? ParentPath { get; set; }
}
