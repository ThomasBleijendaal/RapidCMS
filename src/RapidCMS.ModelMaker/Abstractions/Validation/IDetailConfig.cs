using RapidCMS.ModelMaker.Models.Entities;

namespace RapidCMS.ModelMaker.Core.Abstractions.Validation
{
    public interface IDetailConfig
    {
        /// <summary>
        /// Indicates whether the config should be saved when saving the model.
        /// </summary>
        bool IsEnabled { get; }

        /// <summary>
        /// Indicates whether the editor for this config is visible in the model editor.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        bool IsApplicable(PropertyModel model);

        /// <summary>
        /// Indicates whether the config should always be saved when saving the model.
        /// </summary>
        bool AlwaysIncluded { get; }

        /// <summary>
        /// Indicates the collection alias of the related entity / entities.
        /// </summary>
        string? RelatedCollectionAlias { get; }

        /// <summary>
        /// Name of the FluentValidation validation extension method.
        /// </summary>
        string? ValidationMethodName { get; }

        /// <summary>
        /// Type of data collection to be used from this property.
        /// </summary>
        string? DataCollectionType { get; }
    }
}
