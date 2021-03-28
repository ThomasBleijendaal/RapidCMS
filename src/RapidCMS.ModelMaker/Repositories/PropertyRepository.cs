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

namespace RapidCMS.ModelMaker.Repositories
{
    internal class PropertyRepository : IRepository
    {
        private readonly IModelMakerConfig _config;

        public PropertyRepository(IModelMakerConfig config)
        {
            _config = config;
        }

        public Task AddAsync(IRelated related, string id)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(string id, IParent? parent)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<IEntity>> GetAllAsync(IParent? parent, IQuery query)
        {
            if (parent?.Entity is ModelEntity model)
            {
                return Task.FromResult<IEnumerable<IEntity>>(model.Properties);
            }

            throw new InvalidOperationException();
        }

        public Task<IEnumerable<IEntity>> GetAllNonRelatedAsync(IRelated related, IQuery query)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<IEntity>> GetAllRelatedAsync(IRelated related, IQuery query)
        {
            throw new NotImplementedException();
        }

        public Task<IEntity?> GetByIdAsync(string id, IParent? parent)
        {
            if (parent?.Entity is ModelEntity model &&
                model.Properties.FirstOrDefault(x => x.Id == id) is PropertyModel property)
            {
                return Task.FromResult<IEntity?>(property);
            }

            return Task.FromResult<IEntity?>(default);
        }

        public Task<IEntity?> InsertAsync(IEditContext editContext)
        {
            if (editContext is IEditContext<PropertyModel> typedEditContext &&
                typedEditContext.Parent?.Entity is ModelEntity model)
            {
                var newProperty = typedEditContext.Entity;
                newProperty.Id = Guid.NewGuid().ToString();
                newProperty.Alias = newProperty.Name.ToUrlFriendlyString();
                model.Properties.Add(newProperty);



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



                return Task.FromResult<IEntity?>(newProperty);
            }

            return Task.FromResult<IEntity?>(default);
        }

        public Task<IEntity> NewAsync(IParent? parent, Type? variantType)
        {
            return Task.FromResult<IEntity>(new PropertyModel());
        }

        public Task RemoveAsync(IRelated related, string id)
        {
            throw new NotImplementedException();
        }

        public Task ReorderAsync(string? beforeId, string id, IParent? parent)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(IEditContext editContext)
        {
            throw new NotImplementedException();
        }
    }
}
