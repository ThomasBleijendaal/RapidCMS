using System.Threading.Tasks;
using RapidCMS.Core.Models.Setup;
using RapidCMS.ModelMaker.Abstractions.Validation;

namespace RapidCMS.ModelMaker.Abstractions.Factories
{
    public interface IDataCollectionFactory
    {
        /// <summary>
        /// This method should return the correct RelationSetup for when the user is editing the model with a property which requires an IDataCollection.
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        Task<RelationSetup?> GetModelRelationSetupAsync(IValidatorConfig? config);

        /// <summary>
        /// This method should return the correct RelationSetup (if applicable) when the user is configuring the model in the model maker.
        /// </summary>
        /// <returns></returns>
        Task<RelationSetup?> GetModelEditorRelationSetupAsync();
    }
}
