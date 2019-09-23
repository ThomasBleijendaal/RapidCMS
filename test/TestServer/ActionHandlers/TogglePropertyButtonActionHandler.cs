using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using RapidCMS.Common.ActionHandlers;
using RapidCMS.Common.Authorization;
using RapidCMS.Common.Enums;
using RapidCMS.Common.Forms;
using RapidCMS.Common.Models;
using TestLibrary.Entities;

namespace TestServer.ActionHandlers
{
    public class TogglePropertyButtonActionHandler : IButtonActionHandler
    {
        public TogglePropertyButtonActionHandler()
        {
        }

        public Task ButtonClickAfterRepositoryActionAsync(Button button, EditContext editContext, ButtonContext context)
        {
            return Task.CompletedTask;
        }

        public Task<CrudType> ButtonClickBeforeRepositoryActionAsync(Button button, EditContext editContext, ButtonContext context)
        {
            if (editContext.Entity is ValidationEntity validationEntity)
            {
                validationEntity.Accept = !validationEntity.Accept;
            }

            return Task.FromResult(CrudType.None);
        }

        public OperationAuthorizationRequirement GetOperation(Button button, EditContext editContext)
        {
            return Operations.Read;
        }

        public bool IsCompatible(Button button, EditContext editContext)
        {
            return true;
        }

        public bool RequiresValidForm(Button button, EditContext editContext)
        {
            return false;
        }

        public bool ShouldAskForConfirmation(Button button, EditContext editContext)
        {
            return false;
        }
    }
}
