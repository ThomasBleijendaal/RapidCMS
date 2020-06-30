using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Interactions;
using RapidCMS.Core.Forms;

namespace RapidCMS.Core.Models.Request.Form
{
    public class PersistEntitiesRequestModel : IListButtonInteractionRequestModel
    {
        public ListContext ListContext { get; set; } = default!;
        public string ActionId { get; set; } = default!;
        public object? CustomData { get; set; }
        public IRelated? Related { get; set; } = default!;
    }
}
