using System;
using System.Threading.Tasks;
using RapidCMS.Core.Abstractions.Dispatchers;
using RapidCMS.Core.Abstractions.Factories;
using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Abstractions.Services;
using RapidCMS.Core.Abstractions.State;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Exceptions;
using RapidCMS.Core.Models.Data;
using RapidCMS.Core.Models.Request.Api;
using RapidCMS.Core.Models.Response;
using RapidCMS.Core.Repositories;

namespace RapidCMS.Core.Dispatchers.Api
{
    internal class PersistEntityDispatcher : IInteractionDispatcher<PersistEntityRequestModel, ApiCommandResponseModel>
    {
        private readonly IAuthService _authService;
        private readonly IRepositoryResolver _repositoryResolver;
        private readonly IParentService _parentService;
        private readonly IEditContextFactory _editContextFactory;

        public PersistEntityDispatcher(
            IAuthService authService,
            IRepositoryResolver repositoryResolver,
            IParentService parentService,
            IEditContextFactory editContextFactory)
        {
            _authService = authService;
            _repositoryResolver = repositoryResolver;
            _parentService = parentService;
            _editContextFactory = editContextFactory;
        }

        public async Task<ApiCommandResponseModel> InvokeAsync(PersistEntityRequestModel request, IPageState pageState)
        {
            if (string.IsNullOrWhiteSpace(request.Descriptor.CollectionAlias))
            {
                throw new ArgumentNullException();
            }

            var parent = await _parentService.GetParentAsync(ParentPath.TryParse(request.Descriptor.ParentPath));

            var subjectRepository = _repositoryResolver.GetRepository(request.Descriptor.CollectionAlias);
            var repositoryContext = new RepositoryContext(request.Descriptor.CollectionAlias);
            var referenceEntity = (request.EntityState == EntityState.IsExisting)
                ? await subjectRepository.GetByIdAsync(repositoryContext, request.Descriptor.Id ?? throw new InvalidOperationException("Cannot modify entity without giving an Id."), parent)
                : await subjectRepository.NewAsync(repositoryContext, parent, request.Entity.GetType());

            if (referenceEntity == null)
            {
                throw new NotFoundException("Reference entity is null");
            }

            var usageType = UsageType.Node | (request.EntityState == EntityState.IsNew ? UsageType.New : UsageType.Edit);

            await _authService.EnsureAuthorizedUserAsync(usageType, request.Entity);

            var editContext = _editContextFactory.GetEditContextWrapper(usageType, request.EntityState, request.Entity, referenceEntity, parent);

            try
            {
                if (!editContext.IsValid())
                {
                    throw new InvalidEntityException();
                }

                if (request.EntityState == EntityState.IsNew)
                {
                    return new ApiPersistEntityResponseModel
                    {
                        NewEntity = await subjectRepository.InsertAsync(repositoryContext, editContext)
                    };
                }
                else if (request.EntityState == EntityState.IsExisting)
                {
                    await subjectRepository.UpdateAsync(repositoryContext, editContext);

                    return new ApiCommandResponseModel();
                }
                else
                {
                    throw new InvalidOperationException("Invalid usage type");
                }
            }
            catch (InvalidEntityException)
            {
                return new ApiPersistEntityResponseModel
                {
                    ValidationErrors = editContext.ValidationErrors
                };
            }
        }
    }
}
