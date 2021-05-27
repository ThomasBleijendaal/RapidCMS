using System.Threading.Tasks;
using RapidCMS.Core.Models.Setup;
using RapidCMS.ModelMaker.Abstractions.Validation;

namespace RapidCMS.ModelMaker.Abstractions.Factories
{
    public interface IDataCollectionFactory
    {
        Task<RelationSetup?> GetModelRelationSetupAsync(IValidatorConfig? config);
        Task<RelationSetup?> GetModelEditorRelationSetupAsync();
    }
}
