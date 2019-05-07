using System.Threading.Tasks;
using RapidCMS.Common.Data;
using RapidCMS.Common.Enums;

#nullable enable

namespace RapidCMS.Common.ActionHandlers
{
    public interface IButtonActionHandler
    {
        CrudType GetCrudType();
        bool IsCompatibleWithView(ViewContext viewContext);
        bool ShouldConfirm();
        Task InvokeAsync(string? parentId, string? id, object? customData);
    }
}
