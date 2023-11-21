using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using RapidCMS.Core.Abstractions.Handlers;
using RapidCMS.Core.Authorization;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Forms;
using RapidCMS.Core.Models.Setup;
using RapidCMS.Example.Shared.Data;

namespace RapidCMS.Example.Shared.Handlers;

public class RandomNameActionHandler : IButtonActionHandler
{
    // this method is called just before the IRepository action is performed, based upon the CrudType this button returns
    public Task<CrudType> ButtonClickBeforeRepositoryActionAsync(ButtonSetup button, FormEditContext editContext, ButtonContext context)
    {
        if (editContext.Entity is Tag tag)
        {
            tag.Name = Guid.NewGuid().ToString()[0..6];
        }

        return Task.FromResult(CrudType.None);
    }

    // this method is called after the IRepository action
    public Task ButtonClickAfterRepositoryActionAsync(ButtonSetup button, FormEditContext editContext, ButtonContext context)
    {
        return Task.CompletedTask;
    }

    // this method should return any of the Operations defined in RapidCMS.Common.Authorization which
    // is used to determine whether the user is allow to perform the action this handler does
    public OperationAuthorizationRequirement GetOperation(ButtonSetup button, FormEditContext editContext)
    {
        return Operations.Read;
    }

    // returning false from this method will hide the button, even if it is specified to be drawn on the form
    public bool IsCompatible(ButtonSetup button, FormEditContext editContext)
    {
        return editContext.Entity is Tag;
    }

    // returning true from this method will trigger validation before the click is propagated. if the form is not valid,
    // the click is ignored
    public bool RequiresValidForm(ButtonSetup button, FormEditContext editContext)
    {
        return false;
    }

    // returning true form this method will trigger an confirm dialog before the click is propagated. this allows the user
    // to cancel the action if it is a destructive action
    public bool ShouldAskForConfirmation(ButtonSetup button, FormEditContext editContext)
    {
        return false;
    }
}
