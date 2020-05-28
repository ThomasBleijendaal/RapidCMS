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
    internal class ReorderEntityDispatcher : IInteractionDispatcher<PersistReorderRequestModel, ApiCommandResponseModel>
    {
        private readonly IRepositoryResolver _repositoryResolver;
        private readonly IParentService _parentService;
        private readonly IAuthService _authService;

        public ReorderEntityDispatcher(
            IRepositoryResolver repositoryResolver,
            IParentService parentService,
            IAuthService authService)
        {
            _repositoryResolver = repositoryResolver;
            _parentService = parentService;
            _authService = authService;
        }

        public async Task<ApiCommandResponseModel> InvokeAsync(PersistReorderRequestModel request, IPageState pageState)
        {
            if (string.IsNullOrWhiteSpace(request.Subject.CollectionAlias) ||
                string.IsNullOrWhiteSpace(request.Subject.Id))
            {
                throw new ArgumentNullException();
            }

            var subjectRepository = _repositoryResolver.GetRepository(request.Subject.CollectionAlias);
            var repositoryContext = new RepositoryContext(request.Subject.CollectionAlias);

            var subjectParent = await _parentService.GetParentAsync(ParentPath.TryParse(request.Subject.ParentPath));

            var subjectEntity = await subjectRepository.GetByIdAsync(repositoryContext, request.Subject.Id, subjectParent)
                ?? throw new NotFoundException("Subject entity was not found");

            await _authService.EnsureAuthorizedUserAsync(Operations.Update, subjectEntity);

            await subjectRepository.ReorderAsync(repositoryContext, request.BeforeId, request.Subject.Id, subjectParent);

            return new ApiCommandResponseModel();
        }
    }
}
