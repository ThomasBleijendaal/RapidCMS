using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Forms;
using RapidCMS.Core.Abstractions.Repositories;
using RapidCMS.ModelMaker.Abstractions.Config;
using RapidCMS.Core.Extensions;
using RapidCMS.ModelMaker.Models.Entities;
using RapidCMS.ModelMaker.Abstractions.Validation;
using RapidCMS.ModelMaker.Abstractions.CommandHandlers;
using RapidCMS.ModelMaker.Models.Responses;
using RapidCMS.ModelMaker.Models.Commands;

namespace RapidCMS.ModelMaker.Repositories
{
    internal class PropertyRepository : IRepository
    {
        private readonly IModelMakerConfig _config;
        private readonly ICommandHandler<UpdateRequest<ModelEntity>, ConfirmResponse> _updateEntityCommandHandler;

        public PropertyRepository(
            IModelMakerConfig config,
            ICommandHandler<UpdateRequest<ModelEntity>, ConfirmResponse> updateEntityCommandHandler)
        {
            _config = config;
            _updateEntityCommandHandler = updateEntityCommandHandler;
        }

        public Task AddAsync(IRelated related, string id)
        {
            throw new NotSupportedException();
        }

        public async Task DeleteAsync(string id, IParent? parent)
        {
            if (parent?.Entity is ModelEntity model)
            {
                model.DraftProperties.RemoveAll(x => x.Id == id);

                await _updateEntityCommandHandler.HandleAsync(new UpdateRequest<ModelEntity>(model));
            }
        }

        public Task<IEnumerable<IEntity>> GetAllAsync(IParent? parent, IQuery query)
        {
            if (parent?.Entity is ModelEntity model)
            {
                return Task.FromResult<IEnumerable<IEntity>>(model.DraftProperties);
            }

            throw new InvalidOperationException();
        }

        public Task<IEnumerable<IEntity>> GetAllNonRelatedAsync(IRelated related, IQuery query)
        {
            throw new NotSupportedException();
        }

        public Task<IEnumerable<IEntity>> GetAllRelatedAsync(IRelated related, IQuery query)
        {
            throw new NotSupportedException();
        }

        public Task<IEntity?> GetByIdAsync(string id, IParent? parent)
        {
            if (parent?.Entity is ModelEntity model &&
                model.DraftProperties.FirstOrDefault(x => x.Id == id) is PropertyModel property)
            {
                return Task.FromResult<IEntity?>(property);
            }

            return Task.FromResult<IEntity?>(default);
        }

        public async Task<IEntity?> InsertAsync(IEditContext editContext)
        {
            if (editContext is IEditContext<PropertyModel> typedEditContext &&
                typedEditContext.Parent?.Entity is ModelEntity model)
            {
                var newProperty = typedEditContext.Entity;
                newProperty.Id = Guid.NewGuid().ToString();
                newProperty.Alias = newProperty.Name.ToUrlFriendlyString();
                model.DraftProperties.Add(newProperty);

                if (newProperty.IsTitle)
                {
                    SetPropertyAsOnlyTitle(model, newProperty);
                }

                var property = _config.Properties.FirstOrDefault(x => x.Alias == newProperty.PropertyAlias);

                if (property != null)
                {
                    // TODO: move this to upper repositories
                    var validations = _config.Validators.Where(x => property.Validators.Any(v => v.Alias == x.Alias));

                    foreach (var validation in validations)
                    {
                        var config = Activator.CreateInstance(validation.Config) as IValidatorConfig;

                        var validationModel = Activator.CreateInstance(typeof(PropertyValidationModel<>).MakeGenericType(validation.Config)) as PropertyValidationModel
                            ?? throw new InvalidOperationException("Could not create correct PropertyValidationModel.");

                        validationModel.Alias = validation.Alias;
                        validationModel.Config = config;
                        validationModel.Id = Guid.NewGuid().ToString();

                        newProperty.Validations.Add(validationModel);
                    }
                }

                ValidateProperty(typedEditContext, newProperty, model);

                typedEditContext.EnforceValidEntity();

                await _updateEntityCommandHandler.HandleAsync(new UpdateRequest<ModelEntity>(model));

                return newProperty;
            }

            return default;
        }

        public Task<IEntity> NewAsync(IParent? parent, Type? variantType)
        {
            return Task.FromResult<IEntity>(new PropertyModel());
        }

        public Task RemoveAsync(IRelated related, string id)
        {
            throw new NotSupportedException();
        }

        public Task ReorderAsync(string? beforeId, string id, IParent? parent)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateAsync(IEditContext editContext)
        {
            if (editContext is IEditContext<PropertyModel> typedEditContext &&
                typedEditContext.Parent?.Entity is ModelEntity model)
            {
                model.Alias ??= model.Name.ToUrlFriendlyString();

                var index = model.DraftProperties.FindIndex(x => x.Id == typedEditContext.Entity.Id);

                model.DraftProperties[index] = typedEditContext.Entity;

                if (typedEditContext.Entity.IsTitle)
                {
                    SetPropertyAsOnlyTitle(model, typedEditContext.Entity);
                }

                ValidateProperty(typedEditContext, typedEditContext.Entity, model);

                typedEditContext.EnforceValidEntity();

                await _updateEntityCommandHandler.HandleAsync(new UpdateRequest<ModelEntity>(model));
            }
        }

        private void ValidateProperty(IEditContext<PropertyModel> editContext, PropertyModel property, ModelEntity model)
        {
            if (model.DraftProperties.Count(x => x.Alias == property.Alias) > 1)
            {
                editContext.AddValidationError("Alias", "Alias already used.");
            }
        }

        private static void SetPropertyAsOnlyTitle(ModelEntity model, PropertyModel property)
        {
            model.DraftProperties.Where(x => x.Alias != property.Alias).ForEach(x => x.IsTitle = false);
        }
    }
}
