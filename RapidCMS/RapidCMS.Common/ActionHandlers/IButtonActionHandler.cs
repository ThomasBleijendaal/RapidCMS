using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using RapidCMS.Common.Enums;
using RapidCMS.Common.Forms;
using RapidCMS.Common.Models;

// TODO: make button handlers way more useful:
// 1. Have a InvokeBeforeRepositoryAction + InvokeAfterRepositoryAction
// 2. Pass in the entity for which the button was pressed
// 3. Make this action handler transient and create it everytime something happens to a custom button
// 4. Thightly connect a button handler to a custom button
// 5. Rename the methods to better reflect when they are used

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
