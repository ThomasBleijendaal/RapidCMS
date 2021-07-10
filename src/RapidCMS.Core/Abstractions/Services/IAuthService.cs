using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Setup;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Forms;

namespace RapidCMS.Core.Abstractions.Services
{
    public interface IAuthService
    {
        Task<bool> IsUserAuthorizedAsync(UsageType usageType, IEntity entity);
        Task<bool> IsUserAuthorizedAsync(OperationAuthorizationRequirement operation, IEntity entity);
        Task<bool> IsUserAuthorizedAsync(FormEditContext editContext, IButtonSetup button);

        Task EnsureAuthorizedUserAsync(UsageType usageType, IEntity entity);
        Task EnsureAuthorizedUserAsync(OperationAuthorizationRequirement operation, IEntity entity);
        Task EnsureAuthorizedUserAsync(FormEditContext editContext, IButtonSetup button);
    }
}
