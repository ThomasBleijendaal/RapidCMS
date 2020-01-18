using RapidCMS.Core.Abstractions.Interactions;
using RapidCMS.Core.Forms;

namespace RapidCMS.Core.Models.Request
{
    public class PersistEntityRequestModel : IEditorButtonInteractionRequestModel
    {
        public EditContext EditContext { get; set; } = default!;
        public string ActionId { get; set; } = default!;
        public object? CustomData { get; set; }
    }
}
