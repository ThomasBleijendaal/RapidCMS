using System.Threading.Tasks;
using RapidCMS.Core.Models.Setup;
using RapidCMS.ModelMaker.Abstractions.Factories;
using RapidCMS.ModelMaker.Abstractions.Validation;
using RapidCMS.ModelMaker.DataCollections;
using RapidCMS.ModelMaker.Validation.Config;

namespace RapidCMS.ModelMaker.Factories
{
    internal class LimitedOptionsDataCollectionFactory : IDataCollectionFactory
    {
        public Task<RelationSetup?> GetModelEditorRelationSetupAsync() 
            => Task.FromResult<RelationSetup?>(default);

        public Task<RelationSetup?> GetModelRelationSetupAsync(IValidatorConfig? config)
            => Task.FromResult< RelationSetup?>(new ConcreteDataProviderRelationSetup(new FixedOptionsDataCollection((config as LimitedOptionsValidationConfig)?.Options)));
    }
}
