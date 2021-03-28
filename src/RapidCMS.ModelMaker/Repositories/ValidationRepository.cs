using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Forms;
using RapidCMS.Core.Abstractions.Repositories;
using RapidCMS.ModelMaker.Abstractions.Config;
using RapidCMS.ModelMaker.Abstractions.Validation;
using RapidCMS.ModelMaker.Models.Entities;

namespace RapidCMS.ModelMaker.Repositories
{
    internal class ValidationRepository : IRepository
    {
        private readonly IModelMakerConfig _config;

        public ValidationRepository(IModelMakerConfig config)
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
            if (parent?.Entity is PropertyModel model)
            {
                var models = new List<PropertyValidationModel>();

                var property = _config.Properties.FirstOrDefault(x => x.Alias == model.PropertyAlias);

                if (property != null)
                {
                    // TODO: move this to upper repositories
                    var validations = _config.Validators.Where(x => property.Validators.Any(v => v.Alias == x.Alias));

                    foreach (var validation in validations)
                    {
                        var config = model.Validations.FirstOrDefault(v => v.Alias == validation.Alias)?.Config
                            ?? Activator.CreateInstance(validation.Config) as IValidatorConfig;

                        var validationModel = Activator.CreateInstance(typeof(PropertyValidationModel<>).MakeGenericType(validation.Config)) as PropertyValidationModel
                            ?? throw new InvalidOperationException("Could not create correct PropertyValidationModel.");

                        validationModel.Alias = validation.Alias;
                        validationModel.Config = config;
                        validationModel.Id = Guid.NewGuid().ToString();

                        models.Add(validationModel);
                    }
                }

                return Task.FromResult<IEnumerable<IEntity>>(models);
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
            if (parent?.Entity is PropertyModel model &&
                model.Validations.FirstOrDefault(x => x.Id == id) is PropertyValidationModel property)
            {
                return Task.FromResult<IEntity?>(property);
            }

            return Task.FromResult<IEntity?>(default);
        }

        public Task<IEntity?> InsertAsync(IEditContext editContext)
        {
            throw new NotImplementedException();
        }

        public Task<IEntity> NewAsync(IParent? parent, Type? variantType)
        {
            return Task.FromResult<IEntity>(new PropertyValidationModel());
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
