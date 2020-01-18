using RapidCMS.Core.Forms;

namespace RapidCMS.Core.Abstractions.Interactions
{
    public interface IEditorInListInteractionRequestModel
    {
        string ActionId { get; }
        EditContext EditContext { get; }
        ListContext ListContext { get; }
        object? CustomData { get; }
    }
}
