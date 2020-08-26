using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using RapidCMS.Core.Abstractions.Setup;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Forms;

namespace RapidCMS.Core.Abstractions.Handlers
{
    public interface IButtonActionHandler
    {
        OperationAuthorizationRequirement GetOperation(IButton button, FormEditContext editContext);

        bool IsCompatible(IButton button, FormEditContext editContext);
        bool ShouldAskForConfirmation(IButton button, FormEditContext editContext);
        bool RequiresValidForm(IButton button, FormEditContext editContext);

        Task<CrudType> ButtonClickBeforeRepositoryActionAsync(IButton button, FormEditContext editContext, ButtonContext context);
        Task ButtonClickAfterRepositoryActionAsync(IButton button, FormEditContext editContext, ButtonContext context);
    }
}
