using System;
using System.Linq;
using System.Threading.Tasks;
using RapidCMS.Core.Abstractions.Dispatchers;
using RapidCMS.Core.Abstractions.Factories;
using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Abstractions.Services;
using RapidCMS.Core.Abstractions.State;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Exceptions;
using RapidCMS.Core.Forms;
using RapidCMS.Core.Models.Data;
using RapidCMS.Core.Models.Request.Api;
using RapidCMS.Core.Models.Response;

namespace RapidCMS.Core.Dispatchers.Api
{
    internal class PersistEntityDispatcher : IInteractionDispatcher<PersistEntityRequestModel, ApiCommandResponseModel>
    {
        private readonly IAuthService _authService;
        private readonly IEntityVariantResolver _entityVariantResolver;
        private readonly IRepositoryResolver _repositoryResolver;
        private readonly IParentService _parentService;
        private readonly IEditContextFactory _editContextFactory;

        public PersistEntityDispatcher(
            IAuthService authService,
            IEntityVariantResolver entityVariantResolver,
            IRepositoryResolver repositoryResolver,
            IParentService parentService,
            IEditContextFactory editContextFactory)
        {
            _authService = authService;
            _entityVariantResolver = entityVariantResolver;
            _repositoryResolver = repositoryResolver;
            _parentService = parentService;
            _editContextFactory = editContextFactory;
        }

        public async Task<ApiCommandResponseModel> InvokeAsync(PersistEntityRequestModel request, IPageState pageState)
        {
            if (string.IsNullOrWhiteSpace(request.Descriptor.RepositoryAlias))
            {
                throw new ArgumentNullException();
            }

            var parent = await _parentService.GetParentAsync(ParentPath.TryParse(request.Descriptor.ParentPath));

            var (repositoryEntityType, entityVariants) = _entityVariantResolver.GetValidVariantsForRepository(request.Descriptor.RepositoryAlias);

            if (repositoryEntityType == null || !(entityVariants?.Contains(request.Entity.GetType()) ?? false))
            {
                throw new InvalidOperationException("Invalid entity provided.");
            }

            var subjectRepository = _repositoryResolver.GetRepository(request.Descriptor.RepositoryAlias);
            var referenceEntity = (request.EntityState == EntityState.IsExisting)
                ? await subjectRepository.GetByIdAsync(request.Descriptor.Id ?? throw new InvalidOperationException("Cannot modify entity without giving an Id."), new ViewContext("", parent))
                : await subjectRepository.NewAsync(new ViewContext("", parent), request.Entity.GetType());

            if (referenceEntity == null)
            {
                throw new NotFoundException("Reference entity is null");
            }

            var usageType = UsageType.Node | (request.EntityState == EntityState.IsNew ? UsageType.New : UsageType.Edit);

            await _authService.EnsureAuthorizedUserAsync(usageType, request.Entity);

            var editContext = await _editContextFactory.GetEditContextWrapperAsync(
                usageType,
                request.EntityState,
                repositoryEntityType,
                request.Entity, 
                referenceEntity, 
                parent,
                request.Relations);

            try
            {
                if (!await editContext .IsValidAsync())
                {
                    throw new InvalidEntityException();
                }

                if (request.EntityState == EntityState.IsNew)
                {
                    return new ApiPersistEntityResponseModel
                    {
                        NewEntity = await subjectRepository.InsertAsync(editContext)
                    };
                }
                else if (request.EntityState == EntityState.IsExisting)
                {
                    await subjectRepository.UpdateAsync(editContext);

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
