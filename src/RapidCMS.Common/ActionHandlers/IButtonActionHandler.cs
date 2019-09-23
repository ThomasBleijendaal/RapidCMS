using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using RapidCMS.Common.Enums;
using RapidCMS.Common.Forms;
using RapidCMS.Common.Models;

namespace RapidCMS.Common.ActionHandlers
{
    public interface IButtonActionHandler
    {
        OperationAuthorizationRequirement GetOperation(Button button, EditContext editContext);

        bool IsCompatible(Button button, EditContext editContext);
        bool ShouldAskForConfirmation(Button button, EditContext editContext);
        bool RequiresValidForm(Button button, EditContext editContext);

        Task<CrudType> ButtonClickBeforeRepositoryActionAsync(Button button, EditContext editContext, ButtonContext context);
        Task ButtonClickAfterRepositoryActionAsync(Button button, EditContext editContext, ButtonContext context);
    }
}
