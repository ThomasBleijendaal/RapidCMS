using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Forms;
using RapidCMS.Core.Abstractions.Repositories;
using RapidCMS.Core.Extensions;
using RapidCMS.ModelMaker.Abstractions.Config;
using RapidCMS.ModelMaker.Abstractions.Validation;
using RapidCMS.ModelMaker.Models.Entities;

namespace RapidCMS.ModelMaker.Repositories
{
    internal class ModelMakerRepository : IRepository
    {
        private readonly IModelMakerConfig _config;
        private readonly IServiceProvider _serviceProvider;
        private List<ModelMakerEntity> _entities = new List<ModelMakerEntity>
        {
            new ModelMakerEntity
            {
                Id = "1",
                ModelAlias = "dynamicmodels",
                Data = new Dictionary<string, object?>
                {
                    { "name", "Name1" }
                }
            },
            new ModelMakerEntity
            {
                Id = "2",
                ModelAlias = "dynamicmodels",
                Data = new Dictionary<string, object?>
                {
                    { "name", "Name2" }
                }
            }
        };

        public ModelMakerRepository(
            IModelMakerConfig config,
            IServiceProvider serviceProvider)
        {
            _config = config;
            _serviceProvider = serviceProvider;
        }

        public Task AddAsync(IRelated related, string id)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(string id, IParent? parent)
        {
            _entities.RemoveAll(x => x.Id == id);

            return Task.CompletedTask;
        }

        public Task<IEnumerable<IEntity>> GetAllAsync(IParent? parent, IQuery query)
        {
            return Task.FromResult<IEnumerable<IEntity>>(_entities);
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
            return Task.FromResult(_entities.FirstOrDefault<IEntity>(x => x.Id == id));
        }

        public Task<IEntity?> InsertAsync(IEditContext editContext)
        {
            if (editContext is IEditContext<ModelMakerEntity> typedEditContext)
            {
                var newEntity = typedEditContext.Entity;
                newEntity.Id = $"{_entities.Max(x => int.Parse(x.Id)) + 1}";
                _entities.Add(newEntity);
                return Task.FromResult<IEntity?>(newEntity);
            }

            return Task.FromResult<IEntity?>(default);
        }

        public Task<IEntity> NewAsync(IParent? parent, Type? variantType)
        {
            return Task.FromResult<IEntity>(new ModelMakerEntity());
        }

        public Task RemoveAsync(IRelated related, string id)
        {
            throw new NotImplementedException();
        }

        public Task ReorderAsync(string? beforeId, string id, IParent? parent)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateAsync(IEditContext editContext)
        {
            if (editContext is IEditContext<ModelMakerEntity> typedEditContext)
            {
                var entity = typedEditContext.Entity;

                //var modelDefinition = ConfigurationExtensions.MODELS.First(x => x.Alias == entity.ModelAlias);

                //foreach (var property in modelDefinition.Properties)
                //{
                //    foreach (var validation in property.Validations.Where(x => x.Config?.IsEnabled == true))
                //    {
                //        var validatorConfig = _config.Validators.First(x => x.Alias == validation.Alias);

                //        var validator = _serviceProvider.GetService<IValidator>(validatorConfig.Validator);

                //        if (!await validator.IsValid(entity.Get(property.Alias), validation.Config!))
                //        {
                //            typedEditContext.AddValidationError(property.Alias, await validator.ErrorMessage(validation.Config!));
                //        }
                //    }
                //}

                //editContext.EnforceValidEntity();


            }
        }
    }


}
