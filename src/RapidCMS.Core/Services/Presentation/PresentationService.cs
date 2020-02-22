using System.Collections.Generic;
using System.Threading.Tasks;
using RapidCMS.Core.Abstractions.Dispatchers;
using RapidCMS.Core.Abstractions.Services;
using RapidCMS.Core.Abstractions.Setup;
using RapidCMS.Core.Forms;
using RapidCMS.Core.Models.Request;

namespace RapidCMS.Core.Services.Presentation
{
    internal class PresentationService : IPresentationService
    {
        private readonly IPresenationDispatcher<GetEntitiesRequestModel, ListContext> _getEntitiesDispatcher;
        private readonly IPresenationDispatcher<GetEntityRequestModel, EditContext> _getEntityDispatcher;
        private readonly IPresenationDispatcher<string, IEnumerable<ITypeRegistration>> _getPageDispatcher;

        public PresentationService(
            IPresenationDispatcher<GetEntitiesRequestModel, ListContext> getEntitiesDispatcher,
            IPresenationDispatcher<GetEntityRequestModel, EditContext> getEntityDispatcher,
            IPresenationDispatcher<string, IEnumerable<ITypeRegistration>> getPageDispatcher)
        {
            _getEntitiesDispatcher = getEntitiesDispatcher;
            _getEntityDispatcher = getEntityDispatcher;
            _getPageDispatcher = getPageDispatcher;
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

        public Task<IEnumerable<ITypeRegistration>> GetPageAsync(string pageAlias)
        {
            return _getPageDispatcher.GetAsync(pageAlias);
        }
    }
}
