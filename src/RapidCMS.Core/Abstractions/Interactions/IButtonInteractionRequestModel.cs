using RapidCMS.Core.Forms;

namespace RapidCMS.Core.Abstractions.Interactions
{
    public interface IEditorButtonInteractionRequestModel
    {
        string ActionId { get; }
        FormEditContext EditContext { get; }
        object? CustomData { get; }
    }
}
