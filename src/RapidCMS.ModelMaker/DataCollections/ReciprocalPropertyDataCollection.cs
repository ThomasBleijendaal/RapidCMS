using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Abstractions.Setup;
using RapidCMS.Core.Extensions;
using RapidCMS.Core.Forms;
using RapidCMS.Core.Models.Data;
using RapidCMS.ModelMaker.Abstractions.CommandHandlers;
using RapidCMS.ModelMaker.Abstractions.Config;
using RapidCMS.ModelMaker.Models.Commands;
using RapidCMS.ModelMaker.Models.Entities;
using RapidCMS.ModelMaker.Models.Responses;

namespace RapidCMS.ModelMaker.DataCollections
{
    internal class ReciprocalPropertyDataCollection : IDataCollection
    {
        private readonly ISetupResolver<ICollectionSetup> _collectionSetupResolver;
        private readonly ICommandHandler<GetByIdRequest<ModelEntity>, EntityResponse<ModelEntity>> _modelResolver;
        private PropertyModel? _property;
        private FormEditContext? _editContext;

        public ReciprocalPropertyDataCollection(
            ISetupResolver<ICollectionSetup> collectionSetupResolver,
            ICommandHandler<GetByIdRequest<ModelEntity>, EntityResponse<ModelEntity>> modelResolver)
        {
            _collectionSetupResolver = collectionSetupResolver;
            _modelResolver = modelResolver;
        }
        public void Configure(object configuration) { }


        public event EventHandler? OnDataChange;

        public void Dispose()
        {
            if (_editContext != null)
            {
                _editContext.OnFieldChanged -= EditContext_OnFieldChanged;
            }
        }

        public async Task<IReadOnlyList<IElement>> GetAvailableElementsAsync(IQuery query)
        {
            if (_editContext?.Parent?.Entity is ModelEntity modelEntity)
            {
                var collection = await _collectionSetupResolver.ResolveSetupAsync(modelEntity.Alias);

                var relatedCollectionAlias = _property?.Validations.SelectNotNull(x => x.Config?.RelatedCollectionAlias).FirstOrDefault();
                if (relatedCollectionAlias is string alias)
                {
                    var model = await _modelResolver.HandleAsync(new GetByIdRequest<ModelEntity>(alias));
                    if (model.Entity != null)
                    {
                        var possibleProperties = model.Entity.Properties.Where(x => (x.IsRelationToMany || x.IsRelationToOne) && x.Type == collection.EntityVariant.Type.FullName);

                        return possibleProperties.Select(x => new Element
                        {
                            Id = x.Name,
                            Labels = new[] { x.Name }
                        }).ToList();
                    }
                }
            }

            return new List<IElement>();
        }

        public Task SetEntityAsync(FormEditContext editContext, IParent? parent)
        {
            if (_editContext != null)
            {
                _editContext.OnFieldChanged -= EditContext_OnFieldChanged;
            }

            _editContext = editContext;

            if (_editContext.Entity is PropertyModel property)
            {
                _editContext.OnFieldChanged += EditContext_OnFieldChanged;

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
