using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Forms;
using RapidCMS.Core.Models.Data;
using RapidCMS.ModelMaker.Abstractions.DataCollections;
using RapidCMS.ModelMaker.Abstractions.Validation;
using RapidCMS.ModelMaker.Validation.Config;

namespace RapidCMS.ModelMaker.DataCollections
{
    internal class ModelMakerLimitedOptionsDataCollection : IPropertyDataCollection
    {
        private IEnumerable<string> _options = Enumerable.Empty<string>();

        public event EventHandler? OnDataChange;

        public void Dispose()
        {
            
        }

        public Task<IEnumerable<IElement>> GetAvailableElementsAsync()
        {
            return Task.FromResult<IEnumerable<IElement>>(_options.Select(item => new Element
            {
                Id = item,
                Labels = new[] { item }
            }));
        }

        public Task SetConfigAsync(IValidatorConfig? config)
        {
            if (config is LimitedOptionsValidationConfig limitedOptions)
            {
                _options = limitedOptions.Options;
            }

            return Task.CompletedTask;
        }

        public Task SetEntityAsync(FormEditContext editContext, IParent? parent)
        {
            return Task.CompletedTask;
        }
    }
}
