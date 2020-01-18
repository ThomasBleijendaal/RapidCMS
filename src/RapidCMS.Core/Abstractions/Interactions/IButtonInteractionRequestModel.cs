using RapidCMS.Core.Forms;

namespace RapidCMS.Core.Abstractions.Interactions
{
    public interface IEditorButtonInteractionRequestModel
    {
        string ActionId { get; }
        EditContext EditContext { get; }
        object? CustomData { get; }
    }
}
