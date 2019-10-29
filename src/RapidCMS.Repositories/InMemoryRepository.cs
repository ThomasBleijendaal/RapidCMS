using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RapidCMS.Common.Data;
using RapidCMS.Common.Extensions;
using RapidCMS.Common.Models.Metadata;

namespace RapidCMS.Repositories
{
    /// <summary>
    /// This generic repository saves TEntities in memory and has some basic support for one-to-many relations.
    /// Use *only* List<TRelatedEntity> properties for relations.
    /// </summary>
    /// <typeparam name="TEntity">Entity to store</typeparam>
    public class InMemoryRepository<TEntity> : BaseClassRepository<string, string, TEntity>
        where TEntity : class, IEntity, ICloneable, new()
    {
        private readonly Dictionary<string, List<TEntity>> _data = new Dictionary<string, List<TEntity>>();
        private readonly IServiceProvider _serviceProvider;

        public InMemoryRepository(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
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

            if (query.DataViewExpression != null)
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

        public override Task<TEntity?> GetByIdAsync(string id, string? parentId)
        {
            return Task.FromResult((TEntity?)GetListForParent(parentId).FirstOrDefault(x => x.Id == id).Clone());
        }

        public override async Task<TEntity?> InsertAsync(string? parentId, TEntity entity, IRelationContainer? relations)
        {
            entity.Id = new Random().Next(0, int.MaxValue).ToString();

            await HandleRelationsAsync(entity, relations);

            GetListForParent(parentId).Add(entity);

            return (TEntity)entity.Clone();
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

        public override async Task UpdateAsync(string id, string? parentId, TEntity entity, IRelationContainer? relations)
        {
            var list = GetListForParent(parentId);

            var index = list.FindIndex(x => x.Id == id);

            var newEntity = (TEntity)entity.Clone();

            await HandleRelationsAsync(newEntity, relations);

            list.Insert(index, newEntity);
            list.RemoveAt(index + 1);
        }

        private async Task HandleRelationsAsync(TEntity entity, IRelationContainer? relations)
        {
            // this is some generic code to handle relations very genericly
            // please do not use in production

            if (relations != null)
            {
                foreach (var r in relations.Relations)
                {
                    try
                    {
                        if (r.Property is IFullPropertyMetadata fp)
                        {
                            if (_serviceProvider.GetService(typeof(InMemoryRepository<>).MakeGenericType(r.RelatedEntity)) is IRepository repo)
                            {
                                var relatedEntities = await r.RelatedElements
                                    .Select(x => x.Id.ToString())
                                    .ToListAsync(async id =>
                                    {
                                        var entity = await repo.InternalGetByIdAsync(id, null);
                                        return entity;
                                    });

                                var orignalList = fp.Getter(entity);

                                if (orignalList is IList list)
                                {
                                    list.Clear();

                                    foreach (var relatedEntity in relatedEntities)
                                    {
                                        list.Add(relatedEntity);
                                    }

                                    fp.Setter(entity, list);
                                }
                            }
                        }
                    }
                    catch
                    {
                        // do not care
                    }
                }
            }
        }
    }
}
