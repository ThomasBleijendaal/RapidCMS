using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Forms;
using RapidCMS.Core.Abstractions.Repositories;
using RapidCMS.ModelMaker.Abstractions.Config;
using RapidCMS.ModelMaker.Models.Entities;

namespace RapidCMS.ModelMaker.Repositories
{
    internal class ModelRepository : IRepository
    {
        private readonly IModelMakerConfig _config;

        public ModelRepository(IModelMakerConfig config)
        {
            _config = config;
        }

        public Task AddAsync(IRelated related, string id)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(string id, IParent? parent)
        {
            ConfigurationExtensions.MODELS.RemoveAll(x => x.Id == id);
            return Task.CompletedTask;
        }

        public Task<IEnumerable<IEntity>> GetAllAsync(IParent? parent, IQuery query)
        {
            return Task.FromResult<IEnumerable<IEntity>>(ConfigurationExtensions.MODELS);
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
            return Task.FromResult<IEntity?>(ConfigurationExtensions.MODELS.FirstOrDefault(x => x.Id == id));
        }

        public Task<IEntity?> InsertAsync(IEditContext editContext)
        {
            if (editContext is IEditContext<ModelEntity> typedEditContext)
            {
                var entity = typedEditContext.Entity;
                entity.Id = $"{ConfigurationExtensions.MODELS.Count + 1}";
                ConfigurationExtensions.MODELS.Add(entity);
                return Task.FromResult<IEntity?>(entity);
            }

            return Task.FromResult<IEntity?>(default);
        }

        public Task<IEntity> NewAsync(IParent? parent, Type? variantType)
        {
            return Task.FromResult<IEntity>(new ModelEntity());
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
            return Task.CompletedTask;
        }
    }
}
