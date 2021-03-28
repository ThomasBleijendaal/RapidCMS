using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Forms;
using RapidCMS.Core.Abstractions.Repositories;

namespace RapidCMS.ModelMaker
{
    internal class ModelRepository : IRepository
    {
        private readonly List<ModelEntity> _models = new List<ModelEntity>
        {
            new ModelEntity
            {
                Id = "1",
                Name = "Dynamic model",
                Properties = new List<PropertyModel>
                {
                    new PropertyModel
                    {
                        Id = "1,",
                        EditorAlias = "textbox",
                        Name = "Name",
                        PropertyAlias = "shortstring",
                        Validations = new List<PropertyValidationModel>
                        {
                            new PropertyValidationModel
                            {
                                Id = "1",
                                Alias = "minlength",
                                Config = new MinLengthValidationConfig
                                {
                                    MinLength = 10
                                }
                            }
                        }
                    }
                }
            }
        };

        public Task AddAsync(IRelated related, string id)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(string id, IParent? parent)
        {
            _models.RemoveAll(x => x.Id == id);
            return Task.CompletedTask;
        }

        public Task<IEnumerable<IEntity>> GetAllAsync(IParent? parent, IQuery query)
        {
            return Task.FromResult<IEnumerable<IEntity>>(_models);
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
            return Task.FromResult<IEntity?>(_models.FirstOrDefault(x => x.Id == id));
        }

        public Task<IEntity?> InsertAsync(IEditContext editContext)
        {
            if (editContext is IEditContext<ModelEntity> typedEditContext)
            {
                var entity = typedEditContext.Entity;
                entity.Id = $"{_models.Count + 1}";
                _models.Add(entity);
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
