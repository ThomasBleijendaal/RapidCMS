using System.Collections.Generic;
using System.Threading.Tasks;
using RapidCMS.Core.Abstractions.Setup;
using RapidCMS.Core.Forms;
using RapidCMS.Core.Models.Request;

namespace RapidCMS.Core.Abstractions.Services
{
    public interface IPresentationService
    {
        Task<EditContext> GetEntityAsync(GetEntityRequestModel request);
        Task<ListContext> GetEntitiesAsync(GetEntitiesRequestModel request);
        Task<ListContext> GetEntitiesAsync(GetEntitiesOfParentRequestModel request);

        Task<IEnumerable<ITypeRegistration>> GetPageAsync(string pageAlias);
    }
}
