using System.Threading.Tasks;
using RapidCMS.Common.ActionHandlers;
using RapidCMS.Common.Data;
using RapidCMS.Common.Enums;
using RapidCMS.Common.Forms;

namespace RapidCMS.Common.Models
{
    internal class CustomButton : Button
    {
        internal string Alias { get; set; }

        // TODO: do not resolve this at config time
        internal IButtonActionHandler ActionHandler { get; set; }

        internal override CrudType GetCrudType()
        {
            return ActionHandler.GetCrudType();
        }
        internal override bool IsCompatibleWithForm(EditContext editContext)
        {
            return ActionHandler.IsCompatibleWithForm(editContext);
        }
        internal Task<CrudType?> HandleActionAsync(string? parentId, string? id, object? customData)
        {
            return ActionHandler.InvokeAsync(parentId, id, customData);
        }
    }
}
