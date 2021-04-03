﻿using System;
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
using RapidCMS.Core.Abstractions.Mediators;
using RapidCMS.Core.Models.EventArgs.Mediators;
using RapidCMS.Core.Enums;

namespace RapidCMS.ModelMaker.Repositories
{
    internal class PropertyRepository : IRepository
    {
        private readonly IModelMakerConfig _config;
        private readonly ICommandHandler<UpdateRequest<ModelEntity>, ConfirmResponse> _updateEntityCommandHandler;
        private readonly IMediator _mediator;

        public PropertyRepository(
            IModelMakerConfig config,
            ICommandHandler<UpdateRequest<ModelEntity>, ConfirmResponse> updateEntityCommandHandler,
            IMediator mediator)
        {
            _config = config;
            _updateEntityCommandHandler = updateEntityCommandHandler;
            _mediator = mediator;
        }

        public Task AddAsync(IRelatedViewContext viewContext, string id) => throw new NotSupportedException();

        public async Task DeleteAsync(string id, IViewContext viewContext)
        {
            if (viewContext.Parent?.Entity is ModelEntity model)
            {
                model.DraftProperties.RemoveAll(x => x.Id == id);

                await _updateEntityCommandHandler.HandleAsync(new UpdateRequest<ModelEntity>(model));
            }
        }

        public Task<IEnumerable<IEntity>> GetAllAsync(IViewContext viewContext, IQuery query)
        {
            if (viewContext.Parent?.Entity is ModelEntity model)
            {
                return Task.FromResult<IEnumerable<IEntity>>(model.DraftProperties);
            }

            throw new InvalidOperationException();
        }

        public Task<IEnumerable<IEntity>> GetAllNonRelatedAsync(IRelatedViewContext viewContext, IQuery query) => throw new NotSupportedException();

        public Task<IEnumerable<IEntity>> GetAllRelatedAsync(IRelatedViewContext viewContext, IQuery query) => throw new NotSupportedException();

        public Task<IEntity?> GetByIdAsync(string id, IViewContext viewContext)
        {
            if (viewContext.Parent?.Entity is ModelEntity model &&
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
                model.DraftProperties.Add(newProperty);

                SetDefaultProperties(model, typedEditContext.Entity);

                if (newProperty.IsTitle)
                {
                    SetPropertyAsOnlyTitle(model, newProperty);
                }

                ValidateProperty(typedEditContext, newProperty, model);

                typedEditContext.EnforceValidEntity();

                await _updateEntityCommandHandler.HandleAsync(new UpdateRequest<ModelEntity>(model));

                _mediator.NotifyEvent(this, new RepositoryEventArgs(typeof(ModelRepository), default, model.Id, CrudType.Update));

                return newProperty;
            }

            return default;
        }

        public Task<IEntity> NewAsync(IViewContext viewContext, Type? variantType) => Task.FromResult<IEntity>(new PropertyModel());

        public Task RemoveAsync(IRelatedViewContext viewContext, string id) => throw new NotSupportedException();

        public Task ReorderAsync(string? beforeId, string id, IViewContext viewContext)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateAsync(IEditContext editContext)
        {
            if (editContext is IEditContext<PropertyModel> typedEditContext &&
                typedEditContext.Parent?.Entity is ModelEntity model)
            {
                typedEditContext.Entity.Alias ??= typedEditContext.Entity.Name.ToUrlFriendlyString();

                var index = model.DraftProperties.FindIndex(x => x.Id == typedEditContext.Entity.Id);

                model.DraftProperties[index] = typedEditContext.Entity;

                SetDefaultProperties(model, typedEditContext.Entity);
                if (typedEditContext.Entity.IsTitle)
                {
                    SetPropertyAsOnlyTitle(model, typedEditContext.Entity);
                }

                ValidateProperty(typedEditContext, typedEditContext.Entity, model);

                typedEditContext.EnforceValidEntity();

                await _updateEntityCommandHandler.HandleAsync(new UpdateRequest<ModelEntity>(model));

                _mediator.NotifyEvent(this, new RepositoryEventArgs(typeof(ModelRepository), default, model.Id, CrudType.Update));
            }
        }

        private void ValidateProperty(IEditContext<PropertyModel> editContext, PropertyModel property, ModelEntity model)
        {
            if (model.DraftProperties.Count(x => x.Alias == property.Alias) > 1)
            {
                editContext.AddValidationError("Alias", "Alias already used.");
            }
        }

        private void SetDefaultProperties(ModelEntity model, PropertyModel property)
        {
            if (string.IsNullOrWhiteSpace(property.Alias))
            {
                property.Alias = property.Name.ToUrlFriendlyString();
            }

            var propertyConfig = _config.Properties.FirstOrDefault(x => x.Alias == property.PropertyAlias);
            if (propertyConfig != null)
            {
                var validations = _config.Validators.Where(x => propertyConfig.Validators.Any(v => v.Alias == x.Alias));

                foreach (var validation in validations)
                {
                    var config = property.Validations.FirstOrDefault(x => x.Alias == validation.Alias)?.Config
                        ?? Activator.CreateInstance(validation.Config) as IValidatorConfig;

                    var validationModel = Activator.CreateInstance(typeof(PropertyValidationModel<>).MakeGenericType(validation.Config)) as PropertyValidationModel
                        ?? throw new InvalidOperationException("Could not create correct PropertyValidationModel.");

                    validationModel.Alias = validation.Alias;
                    validationModel.Config = config;
                    validationModel.Id = Guid.NewGuid().ToString();

                    property.Validations.Add(validationModel);
                }
            }
        }

        private static void SetPropertyAsOnlyTitle(ModelEntity model, PropertyModel property)
        {
            model.DraftProperties.Where(x => x.Alias != property.Alias).ForEach(x => x.IsTitle = false);
        }
    }
}