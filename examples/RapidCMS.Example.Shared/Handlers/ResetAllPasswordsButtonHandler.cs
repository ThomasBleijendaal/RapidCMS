using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using RapidCMS.Core.Abstractions.Handlers;
using RapidCMS.Core.Abstractions.Setup;
using RapidCMS.Core.Authorization;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Forms;
using RapidCMS.Core.Models.Setup;

namespace RapidCMS.Example.Shared.Handlers;

public class ResetAllPasswordsButtonHandler : IButtonActionHandler
{
    public OperationAuthorizationRequirement GetOperation(ButtonSetup button, FormEditContext editContext)
    {
        // use the RapidCMS.Core.Authorization.Operations to define what action this button represents
        // and how the authorization handler must challenge the current logged in user with to see whether they
        // can actually perform this action
        return Operations.Delete;
    }

    public bool IsCompatible(ButtonSetup button, FormEditContext editContext)
    {
        // check if the button is compatible with the current view and hide it when it is not compatible
        return true;
    }

    public bool RequiresValidForm(ButtonSetup button, FormEditContext editContext)
    {
        // enables this button to block the action when the form associated with the button is not valid
        return false;
    }

    public bool ShouldAskForConfirmation(ButtonSetup button, FormEditContext editContext)
    {
        // triggers a confirm
        return false;
    }

    public async Task<CrudType> ButtonClickBeforeRepositoryActionAsync(ButtonSetup button, FormEditContext editContext, ButtonContext context)
    {
        await Task.Delay(1000);

        // this dictates how the repository must interpret the button action as after the button has been clicked
        return CrudType.Refresh;
    }

    public Task ButtonClickAfterRepositoryActionAsync(ButtonSetup button, FormEditContext editContext, ButtonContext context)
    {
        // this method is invoked after the repository has performed their action
        return Task.CompletedTask;
    }
}
