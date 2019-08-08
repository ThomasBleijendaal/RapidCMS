using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using RapidCMS.Common.Data;
using TestLibrary.Entities;

namespace TestLibrary.Repositories
{
    public class ValidationRepository : BaseStructRepository<int, int, ValidationEntity>
    {
        private readonly Dictionary<int, ValidationEntity> _data;

        public ValidationRepository(SemaphoreSlim semaphoreSlim) : base(semaphoreSlim)
        {
            _data = new Dictionary<int, ValidationEntity>
            {
                {
                    1,
                    new ValidationEntity {
                        Id = "1",
                        Name = "Name",
                        NotRequired = "A-ok",
                        Range = 10,
                        Accept = false,
                        Textarea = "Some text some text"
                    }
                }
            };
        }

        public override Task DeleteAsync(int id, int? parentId)
        {
            _data.Remove(id);

            return Task.CompletedTask;
        }

        public override Task<IEnumerable<ValidationEntity>> GetAllAsync(int? parentId, IQuery<ValidationEntity> query)
        {
            return Task.FromResult(_data.Where(x => x.Value.ParentId == parentId).Select(x => x.Value));
        }

        public override Task<ValidationEntity> GetByIdAsync(int id, int? parentId)
        {
            return Task.FromResult(_data[id]);
        }

        public override Task<ValidationEntity> InsertAsync(int? parentId, ValidationEntity entity, IRelationContainer? relations)
        {
            entity.Id = (_data.Count + 1).ToString();
            entity.ParentId = parentId;

            _data[_data.Count + 1] = entity;

            return Task.FromResult(_data.Last().Value);
        }

        public override Task<ValidationEntity> NewAsync(int? parentId, Type? variantType = null)
        {
            return Task.FromResult(new ValidationEntity { Id = "", ParentId = parentId });
        }

        public override int ParseKey(string id)
        {
            return int.Parse(id);
        }

        public override int? ParseParentKey(string? parentId)
        {
            return int.TryParse(parentId, out var id) ? id : default(int?);
        }

        public override Task UpdateAsync(int id, int? parentId, ValidationEntity entity, IRelationContainer? relations)
        {
            entity.ParentId = parentId;
            _data[id] = entity;

            return Task.CompletedTask;
        }
    }
}
