using System.Collections.Generic;
using System.Threading.Tasks;
using RapidCMS.Common.Forms;
using RapidCMS.Common.Models;
using RapidCMS.Common.Models.UI;

namespace RapidCMS.Common.Services
{
    internal interface IUIService
    {
        Task<NodeUI> GenerateNodeUIAsync(EditContext editContext, Node nodeEditor);
        Task<ListUI> GenerateListUIAsync(EditContext rootEditContext, IEnumerable<EditContext> editContexts, ListView listView);
        Task<ListUI> GenerateListUIAsync(EditContext rootEditContext, IEnumerable<EditContext> editContexts, ListEditor listEditor);
    }
}
