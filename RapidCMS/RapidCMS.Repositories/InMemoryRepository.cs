using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using RapidCMS.Common.Data;
using RapidCMS.Common.Extensions;

namespace RapidCMS.Repositories
{
    public class InMemoryRepository<TEntity> : BaseClassRepository<string, string, TEntity>
        where TEntity : IEntity, ICloneable, new()
    {
        private readonly Dictionary<string, List<TEntity>> _data = new Dictionary<string, List<TEntity>>();

        public InMemoryRepository(SemaphoreSlim semaphore) : base(semaphore)
        {
        }

        private List<TEntity> GetListForParent(string? parentId)
        {
            var pId = parentId ?? string.Empty;

            if (!_data.ContainsKey(pId))
            {
                _data[pId] = new List<TEntity>();
            }

            return _data[pId];
        }

        public override Task DeleteAsync(string id, string? parentId)
        {
            GetListForParent(parentId).RemoveAll(x => x.Id == id);

            return Task.CompletedTask;
        }

        public override Task<IEnumerable<TEntity>> GetAllAsync(string? parentId, IQuery<TEntity> query)
        {
            var dataQuery = GetListForParent(parentId).AsEnumerable();

            if(query.DataViewExpression != null)
            {
                dataQuery = dataQuery.Where(query.DataViewExpression.Compile());
            }

            if (query.SearchTerm != null)
            {
                // this is not a very useful search function, but it's just an example
                dataQuery = dataQuery.Where(x => x.Id.Contains(query.SearchTerm));
            }

            var data = dataQuery
                .Skip(query.Skip)
                .Take(query.Take)
                .Select(x => (TEntity)x.Clone());

            query.HasMoreData(GetListForParent(parentId).Count > (query.Skip + query.Take));

            return Task.FromResult(data);
        }

        public override Task<TEntity> GetByIdAsync(string id, string? parentId)
        {
            return Task.FromResult((TEntity)GetListForParent(parentId).FirstOrDefault(x => x.Id == id).Clone());
        }

        public override Task<TEntity> InsertAsync(string? parentId, TEntity entity, IRelationContainer? relations)
        {
            entity.Id = new Random().Next(0, int.MaxValue).ToString();

            GetListForParent(parentId).Add(entity);

            return Task.FromResult((TEntity)entity.Clone());
        }

        public override Task<TEntity> NewAsync(string? parentId, Type? variantType = null)
        {
            return Task.FromResult(new TEntity());
        }

        public override string ParseKey(string id)
        {
            return id;
        }

        public override string? ParseParentKey(string? parentId)
        {
            return parentId;
        }

        public override Task UpdateAsync(string id, string? parentId, TEntity entity, IRelationContainer? relations)
        {
            var list = GetListForParent(parentId);

            var index = list.FindIndex(x => x.Id == id);

            list.Insert(index, (TEntity)entity.Clone());
            list.RemoveAt(index + 1);

            return Task.CompletedTask;
        }
    }
}
