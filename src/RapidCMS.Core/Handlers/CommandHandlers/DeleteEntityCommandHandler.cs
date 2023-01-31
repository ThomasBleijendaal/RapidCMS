using System;
using System.Threading.Tasks;
using RapidCMS.Core.Abstractions.Factories;
using RapidCMS.Core.Abstractions.Mediators;
using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Abstractions.Services;
using RapidCMS.Core.Authorization;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Exceptions;
using RapidCMS.Core.Forms;
using RapidCMS.Core.Models.Commands;
using RapidCMS.Core.Models.Data;
using RapidCMS.Core.Models.Response;
using RapidCMS.Core.Resolvers.Repositories;
using RapidCMS.Core.Services.Parent;

namespace RapidCMS.Core.Handlers.CommandHandlers
{
    internal class DeleteEntityCommandHandler : IRequestHandler<DeleteEntityCommand>
    {
        private readonly IAuthService _authService;
        private readonly IConcurrencyService _concurrencyService;
        private readonly IRepositoryResolver _repositoryResolver;

        public DeleteEntityCommandHandler(
            IAuthService authService,
            IConcurrencyService concurrencyService,
            IRepositoryResolver repositoryResolver)
        {
            _authService = authService;
            _concurrencyService = concurrencyService;
            _repositoryResolver = repositoryResolver;
        }

        public async Task<Unit> HandleAsync(DeleteEntityCommand request)
        {
            if (string.IsNullOrWhiteSpace(request.Entity.Id))
            {
                throw new ArgumentException("Id is null");
            }

            var repository = _repositoryResolver.GetRepository(request.RepositoryAlias);
            
            await _authService.EnsureAuthorizedUserAsync(Operations.Delete, request.Entity);

            await _concurrencyService.EnsureCorrectConcurrencyAsync(
                () => repository.DeleteAsync(request.Entity.Id, new ViewContext(null, request.Parent)));

            return Unit.Value;
        }
    }

    internal class UpdateEntityCommandHandler : IRequestHandler<UpdateEntityCommand>
    {
        private readonly IAuthService _authService;
        private readonly IRepositoryResolver _repositoryResolver;

        public UpdateEntityCommandHandler(
            IAuthService authService,
            IRepositoryResolver repositoryResolver)
        {
            _authService = authService;
            _repositoryResolver = repositoryResolver;
        }

        public async Task<Unit> HandleAsync(UpdateEntityCommand request)
        {
            if (string.IsNullOrWhiteSpace(request.Descriptor.RepositoryAlias))
            {
                throw new ArgumentNullException();
            }

            var repository = _repositoryResolver.GetRepository(request.RepositoryAlias);

            await _authService.EnsureAuthorizedUserAsync(Operations.Update, request.Entity);

            try
            {
                await editContext.EnforceCompleteValidationAsync();

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
