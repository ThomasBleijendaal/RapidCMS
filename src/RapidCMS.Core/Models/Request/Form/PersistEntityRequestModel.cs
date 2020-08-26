using RapidCMS.Core.Abstractions.Interactions;
using RapidCMS.Core.Forms;

namespace RapidCMS.Core.Models.Request.Form
{
    public class PersistEntityRequestModel : IEditorButtonInteractionRequestModel
    {
        public FormEditContext EditContext { get; set; } = default!;
        public string ActionId { get; set; } = default!;
        public object? CustomData { get; set; }
    }
}
