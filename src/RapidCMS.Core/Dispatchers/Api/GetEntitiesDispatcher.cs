using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Dispatchers;
using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Abstractions.Services;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Exceptions;
using RapidCMS.Core.Models.Data;
using RapidCMS.Core.Models.Request.Api;
using RapidCMS.Core.Models.Response;

namespace RapidCMS.Core.Dispatchers.Api
{
    internal class GetEntitiesDispatcher : IPresentationDispatcher<GetEntitiesRequestModel, EntitiesResponseModel>
    {
        private readonly IRepositoryResolver _repositoryResolver;
        private readonly IDataViewResolver _dataViewResolver;
        private readonly IParentService _parentService;
        private readonly IAuthService _authService;

        public GetEntitiesDispatcher(
            IRepositoryResolver repositoryResolver,
            IDataViewResolver dataViewResolver,
            IParentService parentService,
            IAuthService authService)
        {
            _repositoryResolver = repositoryResolver;
            _dataViewResolver = dataViewResolver;
            _parentService = parentService;
            _authService = authService;
        }

        public async Task<EntitiesResponseModel> GetAsync(GetEntitiesRequestModel request)
        {
            var subjectRepository = _repositoryResolver.GetRepository(request.RepositoryAlias);

            var parentPath = request is GetEntitiesOfParentRequestModel parentRequest ? parentRequest.ParentPath 
                : request is GetEntitiesOfRelationRequestModel relationRequest ? relationRequest.Related.ParentPath 
                : default;
            var parent = await _parentService.GetParentAsync(ParentPath.TryParse(parentPath));
            var related = default(IRelated);
            if (request is GetEntitiesOfRelationRequestModel relatedRequest)
            {
                var relatedRepository = _repositoryResolver.GetRepository(relatedRequest.Related.RepositoryAlias ?? throw new ArgumentNullException());
                var relatedEntity = await relatedRepository.GetByIdAsync(relatedRequest.Related.Id ?? throw new ArgumentNullException(), default)
                    ?? throw new NotFoundException("Could not find related entity");
                related = new RelatedEntity(parent, relatedEntity, relatedRequest.Related.RepositoryAlias);
            }

            var protoEntity = await subjectRepository.NewAsync(parent, default);

            await _authService.EnsureAuthorizedUserAsync(request.UsageType, protoEntity);
            await _dataViewResolver.ApplyDataViewToQueryAsync(request.Query);

            var action = (request.UsageType & ~(UsageType.List | UsageType.Root | UsageType.NotRoot)) switch
            {
                UsageType.Add when related != null => () => subjectRepository.GetAllNonRelatedAsync(related, request.Query),
                _ when related != null => () => subjectRepository.GetAllRelatedAsync(related, request.Query),
                _ when related == null => () => subjectRepository.GetAllAsync(parent, request.Query),

                _ => default(Func<Task<IEnumerable<IEntity>>>)
            };

            if (action == default)
            {
                throw new InvalidOperationException($"UsageType {request.UsageType} is invalid for this method");
            }

            var entities = await action.Invoke();

            return new EntitiesResponseModel
            {
                Entities = entities,
                MoreDataAvailable = request.Query.MoreDataAvailable
            };
        }
    }
}
