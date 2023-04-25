using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Metadata;
using RapidCMS.Core.Forms;
using RapidCMS.Core.Models.Data;
using RapidCMS.ModelMaker.Validation.Config;

namespace RapidCMS.ModelMaker.DataCollections
{
    public class BooleanLabelDataCollection : IDataCollection
    {
        private BooleanLabelDetailConfig? _config;

        public event EventHandler? OnDataChange;

        public void Configure(object configuration)
        {
            _config = configuration as BooleanLabelDetailConfig;
        }

        public void Dispose()
        {

        }

        public Task<IReadOnlyList<IElement>> GetAvailableElementsAsync(IView view)
            => Task.FromResult<IReadOnlyList<IElement>>(new List<IElement>
            {
                new Element
                {
                    Id = true,
                    Labels = new[] { _config?.Labels.TrueLabel ?? "True" }
                },
                new Element
                {
                    Id = false,
                    Labels = new[] { _config?.Labels.FalseLabel ?? "False" }
                }
            });

        public Task SetEntityAsync(FormEditContext editContext, IPropertyMetadata property, IParent? parent)
            => Task.CompletedTask;
    }
}
