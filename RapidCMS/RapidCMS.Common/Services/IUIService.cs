using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RapidCMS.Common.Data;
using RapidCMS.Common.Models;
using RapidCMS.Common.Models.UI;
using RapidCMS.Common.Validation;

namespace RapidCMS.Common.Services
{
    internal interface IUIService
    {
        Task<NodeUI> GenerateNodeUIAsync(ViewContext viewContext, EditContext editContext, Node nodeEditor);
        Task<ListUI> GenerateListUIAsync(ViewContext listViewContext, EditContext rootEditContext, IEnumerable<EditContext> editContexts, Func<EditContext, ViewContext> entityViewContext, ListView listView);
        Task<ListUI> GenerateListUIAsync(ViewContext listViewContext, EditContext rootEditContext, IEnumerable<EditContext> editContexts, Func<EditContext, ViewContext> entityViewContext, ListEditor listEditor);
    }
}
