using System.Threading.Tasks;
using RapidCMS.Core.Abstractions.Dispatchers;
using RapidCMS.Core.Abstractions.Services;
using RapidCMS.Core.Forms;
using RapidCMS.Core.Models.Request;

namespace RapidCMS.Core.Services.Presentation
{
    internal class PresentationService : IPresentationService
    {
        private readonly IPresenationDispatcher<GetEntitiesRequestModel, ListContext> _getEntitiesDispatcher;
        private readonly IPresenationDispatcher<GetEntityRequestModel, EditContext> _getEntityDispatcher;

        public PresentationService(
            IPresenationDispatcher<GetEntitiesRequestModel, ListContext> getEntitiesDispatcher,
            IPresenationDispatcher<GetEntityRequestModel, EditContext> getEntityDispatcher)
        {
            _getEntitiesDispatcher = getEntitiesDispatcher;
            _getEntityDispatcher = getEntityDispatcher;
        }

        public Task<ListContext> GetEntitiesAsync(GetEntitiesOfParentRequestModel request)
        {
            return _getEntitiesDispatcher.GetAsync(request);
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
