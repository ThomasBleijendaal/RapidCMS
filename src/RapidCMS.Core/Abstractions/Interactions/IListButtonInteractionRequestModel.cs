using RapidCMS.Core.Forms;

namespace RapidCMS.Core.Abstractions.Interactions;

public interface IListButtonInteractionRequestModel
{
    string ActionId { get; }
    ListContext ListContext { get; }
    object? CustomData { get; }
}
