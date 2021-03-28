using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Forms;
using RapidCMS.Core.Models.Data;
using RapidCMS.ModelMaker.Abstractions.Config;

namespace RapidCMS.ModelMaker.DataCollections
{
    public class PropertyTypeDataCollection : IDataCollection
    {
        private readonly IModelMakerConfig _config;

        public PropertyTypeDataCollection(IModelMakerConfig config)
        {
            _config = config;
        }

        public event EventHandler? OnDataChange;

        public void Dispose()
        {
        }

        public Task<IEnumerable<IElement>> GetAvailableElementsAsync()
        {
            return Task.FromResult<IEnumerable<IElement>>(
                _config.Properties.Select(x => new Element
                {
                    Id = x.Alias,
                    Labels = new[] { x.Name }
                }));
        }

        public Task SetEntityAsync(FormEditContext editContext, IParent? parent)
        {
            return Task.CompletedTask;
        }
    }
}
