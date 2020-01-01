using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using RapidCMS.Core.Abstractions.Setup;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Forms;

namespace RapidCMS.Core.Abstractions.Handlers
{
    public interface IButtonActionHandler
    {
        OperationAuthorizationRequirement GetOperation(IButton button, EditContext editContext);

        bool IsCompatible(IButton button, EditContext editContext);
        bool ShouldAskForConfirmation(IButton button, EditContext editContext);
        bool RequiresValidForm(IButton button, EditContext editContext);

        Task<CrudType> ButtonClickBeforeRepositoryActionAsync(IButton button, EditContext editContext, ButtonContext context);
        Task ButtonClickAfterRepositoryActionAsync(IButton button, EditContext editContext, ButtonContext context);
    }
}
