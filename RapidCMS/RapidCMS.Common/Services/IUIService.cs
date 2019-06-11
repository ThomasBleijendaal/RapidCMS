using System;
using System.Threading.Tasks;
using RapidCMS.Common.Data;
using RapidCMS.Common.Models;
using RapidCMS.Common.Models.UI;


namespace RapidCMS.Common.Services
{
    internal interface IUIService
    {
        Task<NodeUI> GenerateNodeUIAsync(ViewContext viewContext, Node nodeEditor);
        Task<ListUI> GenerateListUIAsync(ViewContext listViewContext, Func<UISubject, ViewContext> entityViewContext, ListView listView);
        Task<ListUI> GenerateListUIAsync(ViewContext listViewContext, Func<UISubject, ViewContext> entityViewContext, ListEditor listEditor);
    }
}
