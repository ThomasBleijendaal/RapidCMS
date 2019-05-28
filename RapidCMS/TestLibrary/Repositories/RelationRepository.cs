using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RapidCMS.Common.Data;
using TestLibrary.Entities;

#nullable enable

namespace TestLibrary.Repositories
{
    public class RelationRepository : BaseStructRepository<int, int, RelationEntity>
    {
        private readonly List<RelationEntity> _data = new List<RelationEntity>
        {
            new RelationEntity
            {
                RealId = 1,
                Name = "No relation"
            }
        };

        protected string Name => "Relation Repo";

        public override async Task<IEnumerable<RelationEntity>> GetAllAsync(int? parentId)
        {
            await Task.Delay(1);

            return _data;
        }

        public override async Task<RelationEntity> GetByIdAsync(int id, int? parentId)
        {
            await Task.Delay(1);

            return _data.FirstOrDefault(x => x.RealId == id);
        }

        public override async Task<RelationEntity> InsertAsync(int? parentId, RelationEntity entity, IRelationContainer relations)
        {
            await Task.Delay(1);

            entity.RealId = _data.Max(x => x.RealId) + 1;
            entity.AzureTableStorageEntityIds = relations
                .GetRelatedElementIdsFor<AzureTableStorageEntity, string>()
                .ToList();

            _data.Add(entity);

            return entity;
        }

        public override async Task UpdateAsync(int id, int? parentId, RelationEntity entity, IRelationContainer relations)
        {
            await Task.Delay(1);

            var element = _data.First(x => x.RealId == id);

            element.Name = entity.Name;
            element.AzureTableStorageEntityId = entity.AzureTableStorageEntityId;
            element.Location = entity.Location;
            entity.AzureTableStorageEntityIds = relations
                .GetRelatedElementIdsFor<AzureTableStorageEntity, string>()
                .ToList();
        }

        public override Task<RelationEntity> NewAsync(int? parentId, Type? variantType)
        {
            return Task.FromResult(new RelationEntity
            {

            });
        }

        public override async Task DeleteAsync(int id, int? parentId)
        {
            await Task.Delay(1);

            _data.RemoveAll(x => x.RealId == id);
        }

        public override int ParseKey(string id)
        {
            return int.TryParse(id, out var intId) ? intId : 0;
        }

        public override int? ParseParentKey(string? parentId)
        {
            return int.TryParse(parentId, out var intParentId)
                    ? intParentId
                    : default(int?);
        }
    }
}
