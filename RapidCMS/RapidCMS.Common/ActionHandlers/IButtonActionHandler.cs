using System.Threading.Tasks;
using RapidCMS.Common.Data;
using RapidCMS.Common.Enums;
using RapidCMS.Common.Forms;

namespace RapidCMS.Common.ActionHandlers
{
    public interface IButtonActionHandler
    {
        CrudType GetCrudType();
        bool IsCompatibleWithForm(EditContext editContext);
        bool ShouldConfirm();
        Task InvokeAsync(string? parentId, string? id, object? customData);
        bool RequiresValidForm();
    }
}
