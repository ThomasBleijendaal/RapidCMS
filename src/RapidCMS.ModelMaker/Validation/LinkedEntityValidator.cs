using System.Threading.Tasks;
using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Abstractions.Setup;
using RapidCMS.Core.Forms;
using RapidCMS.ModelMaker.Validation.Base;
using RapidCMS.ModelMaker.Validation.Config;

namespace RapidCMS.ModelMaker.Validation
{
    public class LinkedEntityValidator : BaseValidator<string, LinkedEntityValidationConfig>
    {
        private readonly ISetupResolver<ICollectionSetup> _collectionSetupResolver;
        private readonly IRepositoryResolver _repositoryResolver;

        public LinkedEntityValidator(
            ISetupResolver<ICollectionSetup> collectionSetupResolver,
            IRepositoryResolver repositoryResolver)
        {
            _collectionSetupResolver = collectionSetupResolver;
            _repositoryResolver = repositoryResolver;
        }

        protected override Task<string> ErrorMessage(LinkedEntityValidationConfig validatorConfig)
        {
            return Task.FromResult("Selected entity not valid.");
        }

        protected override async Task<bool> IsValid(string? value, LinkedEntityValidationConfig validatorConfig)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return true;
            } 


            var collectionSetup = await _collectionSetupResolver.ResolveSetupAsync(validatorConfig.CollectionAlias);
            var repository = _repositoryResolver.GetRepository(collectionSetup);

            var entity = await repository.GetByIdAsync(value, ViewContext.Default);

            return entity != null;
        }
    }
}
