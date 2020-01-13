using System.Threading.Tasks;
using RapidCMS.Core.Abstractions.Dispatchers;
using RapidCMS.Core.Abstractions.Services;
using RapidCMS.Core.Forms;
using RapidCMS.Core.Models.Request;

namespace RapidCMS.Core.Services.Presentation
{
    internal class PresentationService : IPresentationService
    {
        private readonly IPresenationDispatcher<GetEntityRequestModel, EditContext> _getEntityDispatcher;
        private readonly IPresenationDispatcher<GetEntitiesRequestModel, ListContext> _getEntitiesDispatcher;

        public PresentationService(
            IPresenationDispatcher<GetEntityRequestModel, EditContext> getEntityDispatcher,
            IPresenationDispatcher<GetEntitiesRequestModel, ListContext> getEntitiesDispatcher)
        {
            _getEntityDispatcher = getEntityDispatcher;
            _getEntitiesDispatcher = getEntitiesDispatcher;
        }

        public Task<ListContext> GetEntitiesAsync(GetEntitiesRequestModel request)
        {
            return _getEntitiesDispatcher.GetAsync(request);
        }

        public Task<EditContext> GetEntityAsync(GetEntityRequestModel request)
        {
            return _getEntityDispatcher.GetAsync(request);
        }
    }
}
