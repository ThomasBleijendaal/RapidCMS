using System.Threading.Tasks;
using RapidCMS.Core.Models.Setup;
using RapidCMS.Core.Providers;
using RapidCMS.ModelMaker.Core.Abstractions.Factories;
using RapidCMS.ModelMaker.Core.Abstractions.Validation;
using RapidCMS.ModelMaker.Validation.Config;

namespace RapidCMS.ModelMaker.Factories
{
    internal class BooleanLabelDataCollectionFactory : IDataCollectionFactory
    {
        public Task<RelationSetup?> GetModelEditorRelationSetupAsync()
            => Task.FromResult<RelationSetup?>(default);

        public Task<RelationSetup?> GetModelRelationSetupAsync(IValidatorConfig? config)
        {
            var labels = (config as BooleanLabelValidationConfig)?.Labels;
            if (labels == null)
            {
                return Task.FromResult<RelationSetup?>(default);
            }

            return Task.FromResult<RelationSetup?>(
                new ConcreteDataProviderRelationSetup(
                    new FixedOptionsDataProvider(
                        new (object, string)[]
                        {
                            (true, labels.TrueLabel ?? "True"),
                            (false, labels.FalseLabel ?? "False")
                        })));
        }
    }
}
