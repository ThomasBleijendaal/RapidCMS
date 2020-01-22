using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using RapidCMS.Core.Abstractions.Dispatchers;
using RapidCMS.Core.Abstractions.Services;
using RapidCMS.Core.Forms;
using RapidCMS.Core.Models.Request;

namespace RapidCMS.Core.Services.Presentation
{
    internal class PresentationService : IPresentationService
    {
        private readonly IServiceProvider _serviceProvider;

        public PresentationService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Task<ListContext> GetEntitiesAsync(GetEntitiesOfParentRequestModel request)
        {
            return _serviceProvider.GetService<IPresenationDispatcher<GetEntitiesRequestModel, ListContext>>().GetAsync(request);
        }

        public Task<ListContext> GetEntitiesAsync(GetEntitiesRequestModel request)
        {
            return _serviceProvider.GetService<IPresenationDispatcher<GetEntitiesRequestModel, ListContext>>().GetAsync(request);
        }

        public Task<EditContext> GetEntityAsync(GetEntityRequestModel request)
        {
            return _serviceProvider.GetService<IPresenationDispatcher<GetEntityRequestModel, EditContext>>().GetAsync(request);
        }
    }
}
