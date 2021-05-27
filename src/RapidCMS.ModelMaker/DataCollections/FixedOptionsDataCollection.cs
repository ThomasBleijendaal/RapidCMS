using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Forms;
using RapidCMS.Core.Models.Data;

namespace RapidCMS.ModelMaker.DataCollections
{
    internal class FixedOptionsDataCollection : IDataCollection
    {
        private readonly IEnumerable<string> _options;

        public FixedOptionsDataCollection(IEnumerable<string>? options)
        {
            _options = options ?? Enumerable.Empty<string>();
        }

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

        public Task SetEntityAsync(FormEditContext editContext, IParent? parent)
        {
            return Task.CompletedTask;
        }
    }
}
