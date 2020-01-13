using System.Collections.Generic;
using System.Threading.Tasks;
using RapidCMS.Core.Forms;
using RapidCMS.Core.Models.Request;

namespace RapidCMS.Core.Abstractions.Services
{
    public interface IPresentationService
    {
        Task<EditContext> GetEntityAsync(GetEntityRequestModel request);
        Task<ListContext> GetEntitiesAsync(GetEntitiesRequestModel request);
    }
}
