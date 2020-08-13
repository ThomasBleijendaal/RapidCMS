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

namespace RapidCMS.Core.Dispatchers.Api
{
    internal class RelateEntityDispatcher : IInteractionDispatcher<PersistRelatedEntityRequestModel, ApiCommandResponseModel>
    {
        private readonly IRepositoryResolver _repositoryResolver;
        private readonly IAuthService _authService;

        public RelateEntityDispatcher(
            IRepositoryResolver repositoryResolver,
            IAuthService authService)
        {
            _repositoryResolver = repositoryResolver;
            _authService = authService;
        }

        public async Task<ApiCommandResponseModel> InvokeAsync(PersistRelatedEntityRequestModel request, IPageState pageState)
        {
            if (string.IsNullOrWhiteSpace(request.Subject.RepositoryAlias) ||
                string.IsNullOrWhiteSpace(request.Subject.Id) ||
                string.IsNullOrWhiteSpace(request.Related.RepositoryAlias) ||
                string.IsNullOrWhiteSpace(request.Related.Id))
            {
                throw new ArgumentNullException();
            }

            var subjectRepository = _repositoryResolver.GetRepository(request.Subject.RepositoryAlias);
            var relatedRepository = _repositoryResolver.GetRepository(request.Related.RepositoryAlias);

            var subjectEntity = await subjectRepository.GetByIdAsync(request.Subject.Id, default)
                ?? throw new NotFoundException("Subject entity was not found");
            var relatedEntity = await relatedRepository.GetByIdAsync(request.Related.Id, default)
                ?? throw new NotFoundException("Related entity was not found");

            var related = new RelatedEntity(relatedEntity, request.Related.RepositoryAlias);

            if (request.Action == PersistRelatedEntityRequestModel.Actions.Add)
            {
                await _authService.EnsureAuthorizedUserAsync(Operations.Add, subjectEntity);
                await subjectRepository.AddAsync(related, request.Subject.Id);
            }
            else if (request.Action == PersistRelatedEntityRequestModel.Actions.Remove)
            {
                await _authService.EnsureAuthorizedUserAsync(Operations.Remove, subjectEntity);
                await subjectRepository.AddAsync(related, request.Subject.Id);
            }

            return new ApiCommandResponseModel();
        }
    }
}
