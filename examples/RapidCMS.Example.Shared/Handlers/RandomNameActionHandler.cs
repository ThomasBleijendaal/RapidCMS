using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using RapidCMS.Core.Abstractions.Handlers;
using RapidCMS.Core.Abstractions.Setup;
using RapidCMS.Core.Authorization;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Forms;
using RapidCMS.Example.Shared.Data;

namespace RapidCMS.Example.Shared.Handlers
{
    public class RandomNameActionHandler : IButtonActionHandler
    {
        // this method is called just before the IRepository action is performed, based upon the CrudType this button returns
        public Task<CrudType> ButtonClickBeforeRepositoryActionAsync(IButton button, EditContext editContext, ButtonContext context)
        {
            if (editContext.Entity is Tag tag)
            {
                tag.Name = Guid.NewGuid().ToString()[0..6];
            }

            return Task.FromResult(CrudType.None);
        }

        // this method is called after the IRepository action
        public Task ButtonClickAfterRepositoryActionAsync(IButton button, EditContext editContext, ButtonContext context)
        {
            return Task.CompletedTask;
        }

        // this method should return any of the Operations defined in RapidCMS.Common.Authorization which
        // is used to determine whether the user is allow to perform the action this handler does
        public OperationAuthorizationRequirement GetOperation(IButton button, EditContext editContext)
        {
            return Operations.Read;
        }

        // returning false from this method will hide the button, even if it is specified to be drawn on the form
        public bool IsCompatible(IButton button, EditContext editContext)
        {
            return editContext.Entity is Tag;
        }

        // returning true from this method will trigger validation before the click is propagated. if the form is not valid,
        // the click is ignored
        public bool RequiresValidForm(IButton button, EditContext editContext)
        {
            return false;
        }

        // returning true form this method will trigger an confirm dialog before the click is propagated. this allows the user
        // to cancel the action if it is a destructive action
        public bool ShouldAskForConfirmation(IButton button, EditContext editContext)
        {
            return false;
        }
    }
}
