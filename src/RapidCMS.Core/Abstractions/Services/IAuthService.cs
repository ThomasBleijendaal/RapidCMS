using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Forms;
using RapidCMS.Core.Models.Setup;

namespace RapidCMS.Core.Abstractions.Services
{
    internal interface IAuthService
    {
        Task EnsureAuthorizedUserAsync(UsageType usageType, IEntity entity);
        Task EnsureAuthorizedUserAsync(OperationAuthorizationRequirement operation, IEntity entity);
        Task EnsureAuthorizedUserAsync(EditContext editContext, ButtonSetup button);
    }
}
