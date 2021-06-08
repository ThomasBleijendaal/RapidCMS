using System.Collections.Generic;
using System.Threading.Tasks;
using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Abstractions.Setup;
using RapidCMS.Core.Forms;
using RapidCMS.ModelMaker.Validation.Base;
using RapidCMS.ModelMaker.Validation.Config;

namespace RapidCMS.ModelMaker.Validation
{
    public class LinkedEntitiesValidator : BaseValidator<List<string>, LinkedEntityValidationConfig>
    {
        private readonly ISetupResolver<ICollectionSetup> _collectionSetupResolver;
        private readonly IRepositoryResolver _repositoryResolver;

        public LinkedEntitiesValidator(
            ISetupResolver<ICollectionSetup> collectionSetupResolver,
            IRepositoryResolver repositoryResolver)
        {
            _collectionSetupResolver = collectionSetupResolver;
            _repositoryResolver = repositoryResolver;
        }

        protected override Task<string> ErrorMessage(LinkedEntityValidationConfig validatorConfig)
        {
            return Task.FromResult("Selected entities not valid.");
        }

        protected override async Task<bool> IsValid(List<string>? value, LinkedEntityValidationConfig validatorConfig)
        {
            if (value == null)
            {
                return true;
            }


            var collectionSetup = await _collectionSetupResolver.ResolveSetupAsync(validatorConfig.CollectionAlias);
            var repository = _repositoryResolver.GetRepository(collectionSetup);

            foreach (var element in value)
            {
                var entity = await repository.GetByIdAsync(element, ViewContext.Default);
                if (entity == null)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
