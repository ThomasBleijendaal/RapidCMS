using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Forms;
using RapidCMS.Core.Models.Data;
using RapidCMS.ModelMaker.Abstractions.Config;
using RapidCMS.ModelMaker.Models.Entities;

namespace RapidCMS.ModelMaker.DataCollections
{
    public class PropertyEditorDataCollection : IDataCollection
    {
        private readonly IModelMakerConfig _config;
        private PropertyModel? _property;
        private FormEditContext? _editContext;

        public PropertyEditorDataCollection(IModelMakerConfig config)
        {
            _config = config;
        }

        public event EventHandler? OnDataChange;

        public void Dispose()
        {
            if (_editContext != null)
            {
                _editContext.OnFieldChanged -= EditContext_OnFieldChanged;
            }
        }

        public Task<IEnumerable<IElement>> GetAvailableElementsAsync()
        {
            var property = _config.Properties.FirstOrDefault(x => x.Alias == _property?.PropertyAlias);

            return Task.FromResult<IEnumerable<IElement>>(
                _config.Editors
                    .Where(e => property?.Editors.Any(pe => pe.Alias == e.Alias) ?? false)
                    .Select(x => new Element
                    {
                        Id = x.Alias,
                        Labels = new[]
                        {
                            x.Name
                        }
                    }));
        }

        public Task SetEntityAsync(FormEditContext editContext, IParent? parent)
        {
            if (_editContext != null)
            {
                _editContext.OnFieldChanged -= EditContext_OnFieldChanged;
            }

            _editContext = editContext;

            if (editContext.Entity is PropertyModel property)
            {
                editContext.OnFieldChanged += EditContext_OnFieldChanged;

                _property = property;
            }

            return Task.CompletedTask;
        }

        private void EditContext_OnFieldChanged(object? sender, FieldChangedEventArgs e)
        {
            OnDataChange?.Invoke(this, new EventArgs());
        }
    }
}
