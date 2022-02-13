using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Forms;
using RapidCMS.Core.Models.Setup;

namespace RapidCMS.Core.Abstractions.Handlers
{
    public interface IButtonSetupActionHandler
    {
        OperationAuthorizationRequirement GetOperation(ButtonSetup button, FormEditContext editContext);

        bool IsCompatible(ButtonSetup button, FormEditContext editContext);
        bool ShouldAskForConfirmation(ButtonSetup button, FormEditContext editContext);
        bool RequiresValidForm(ButtonSetup button, FormEditContext editContext);

        Task<CrudType> ButtonClickBeforeRepositoryActionAsync(ButtonSetup button, FormEditContext editContext, ButtonContext context);
        Task ButtonClickAfterRepositoryActionAsync(ButtonSetup button, FormEditContext editContext, ButtonContext context);
    }
}
