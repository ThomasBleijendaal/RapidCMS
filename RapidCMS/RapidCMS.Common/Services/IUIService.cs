using System;
using System.Threading.Tasks;
using RapidCMS.Common.Data;
using RapidCMS.Common.Models;
using RapidCMS.Common.Models.UI;

#nullable enable

namespace RapidCMS.Common.Services
{
    internal interface IUIService
    {
        Task<NodeUI> GenerateNodeUIAsync(ViewContext viewContext, NodeEditor nodeEditor);
        ListUI GenerateListUI(ViewContext listViewContext, Func<UISubject, ViewContext> entityViewContext, ListView listView);
        ListUI GenerateListUI(ViewContext listViewContext, Func<UISubject, ViewContext> entityViewContext, ListEditor listEditor);
    }
}
