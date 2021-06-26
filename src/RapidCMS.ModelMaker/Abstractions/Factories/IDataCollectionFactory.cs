using System;
using System.Threading.Tasks;
using RapidCMS.Core.Models.Setup;
using RapidCMS.ModelMaker.Core.Abstractions.Validation;

namespace RapidCMS.ModelMaker.Core.Abstractions.Factories
{
    // TODO: refactor
    [Obsolete]
    public interface IDataCollectionFactory
    {
        /// <summary>
        /// This method should return the correct RelationSetup for when the user is editing the model with a property which requires an IDataCollection.
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        Task<RelationSetup?> GetModelRelationSetupAsync(IDetailConfig config);

        /// <summary>
        /// This method should return the correct RelationSetup (if applicable) when the user is configuring the model in the model maker.
        /// </summary>
        /// <returns></returns>
        Task<RelationSetup?> GetModelEditorRelationSetupAsync();
    }
}
