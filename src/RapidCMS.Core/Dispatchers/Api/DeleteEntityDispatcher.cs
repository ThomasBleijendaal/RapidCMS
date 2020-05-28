using System;
using System.Threading.Tasks;
using RapidCMS.Core.Abstractions.Dispatchers;
using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Abstractions.Services;
using RapidCMS.Core.Abstractions.State;
using RapidCMS.Core.Authorization;
using RapidCMS.Core.Exceptions;
using RapidCMS.Core.Models.Data;
using RapidCMS.Core.Models.Request.Api;
using RapidCMS.Core.Models.Response;
using RapidCMS.Core.Repositories;

namespace RapidCMS.Core.Dispatchers.Api
{
    internal class DeleteEntityDispatcher : IInteractionDispatcher<DeleteEntityRequestModel, ApiCommandResponseModel>
    {
        private readonly IAuthService _authService;
        private readonly IRepositoryResolver _repositoryResolver;
        private readonly IParentService _parentService;

        public DeleteEntityDispatcher(
            IAuthService authService,
            IRepositoryResolver repositoryResolver,
            IParentService parentService)
        {
            _authService = authService;
            _repositoryResolver = repositoryResolver;
            _parentService = parentService;
        }

        public async Task<ApiCommandResponseModel> InvokeAsync(DeleteEntityRequestModel request, IPageState pageState)
        {
            if (string.IsNullOrWhiteSpace(request.Descriptor.CollectionAlias) ||
                string.IsNullOrWhiteSpace(request.Descriptor.Id))
            {
                throw new ArgumentNullException();
            }

            var parent = await _parentService.GetParentAsync(ParentPath.TryParse(request.Descriptor.ParentPath));

            var subjectRepository = _repositoryResolver.GetRepository(request.Descriptor.CollectionAlias);
            var repositoryContext = new RepositoryContext(request.Descriptor.CollectionAlias);
            var subjectEntity = await subjectRepository.GetByIdAsync(repositoryContext, request.Descriptor.Id, parent) 
                ?? throw new NotFoundException("Could not find entity to delete");

            await _authService.EnsureAuthorizedUserAsync(Operations.Delete, subjectEntity);

            await subjectRepository.DeleteAsync(repositoryContext, request.Descriptor.Id, parent);

            return new ApiCommandResponseModel();
        }
    }
}
