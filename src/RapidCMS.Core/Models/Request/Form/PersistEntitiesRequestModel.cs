using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Interactions;
using RapidCMS.Core.Forms;
using RapidCMS.Core.Navigation;

namespace RapidCMS.Core.Models.Request.Form
{
    public class PersistEntitiesRequestModel : IListButtonInteractionRequestModel
    {
        public ListContext ListContext { get; set; } = default!;
        public NavigationState NavigationState { get; set; } = default!;
        public string ActionId { get; set; } = default!;
        public object? CustomData { get; set; }
        public IRelated? Related { get; set; } = default!;
    }
}
