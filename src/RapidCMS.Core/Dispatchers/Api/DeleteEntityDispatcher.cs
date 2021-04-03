using System;
using System.Threading.Tasks;
using RapidCMS.Core.Abstractions.Dispatchers;
using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Abstractions.Services;
using RapidCMS.Core.Abstractions.State;
using RapidCMS.Core.Authorization;
using RapidCMS.Core.Exceptions;
using RapidCMS.Core.Forms;
using RapidCMS.Core.Models.Data;
using RapidCMS.Core.Models.Request.Api;
using RapidCMS.Core.Models.Response;

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
            if (string.IsNullOrWhiteSpace(request.Descriptor.RepositoryAlias) ||
                string.IsNullOrWhiteSpace(request.Descriptor.Id))
            {
                throw new ArgumentNullException();
            }

            var parent = await _parentService.GetParentAsync(ParentPath.TryParse(request.Descriptor.ParentPath));

            var subjectRepository = _repositoryResolver.GetRepository(request.Descriptor.RepositoryAlias);
            var subjectEntity = await subjectRepository.GetByIdAsync(request.Descriptor.Id, new ViewContext("", parent))
                ?? throw new NotFoundException("Could not find entity to delete");

            await _authService.EnsureAuthorizedUserAsync(Operations.Delete, subjectEntity);

            await subjectRepository.DeleteAsync(request.Descriptor.Id, new ViewContext("", parent));

            return new ApiCommandResponseModel();
        }
    }
}
