﻿using RapidCMS.ModelMaker.Models.Entities;

namespace RapidCMS.ModelMaker.Abstractions.Validation
{
    public interface IValidatorConfig
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
    }
}
