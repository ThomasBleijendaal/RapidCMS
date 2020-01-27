using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Forms;
using RapidCMS.Core.Abstractions.Metadata;
using RapidCMS.Core.Abstractions.Repositories;
using RapidCMS.Core.Extensions;
using RapidCMS.Core.Repositories;

namespace RapidCMS.Repositories
{
    /// <summary>
    /// This generic repository saves TEntities in memory and has some basic support for one-to-many relations.
    /// Use *only* List<TRelatedEntity> properties for relations.
    /// </summary>
    /// <typeparam name="TEntity">Entity to store</typeparam>
    public class InMemoryRepository<TEntity> : BaseRepository<TEntity>
        where TEntity : class, IEntity, ICloneable, new()
    {
        protected Dictionary<string, List<TEntity>> _data = new Dictionary<string, List<TEntity>>();
        private readonly IServiceProvider _serviceProvider;

        public InMemoryRepository(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        private List<TEntity> GetListForParent(IParent? parent)
        {
            var pId = parent?.Entity.Id ?? string.Empty;

            if (!_data.ContainsKey(pId))
            {
                _data[pId] = new List<TEntity>();
            }

            return _data[pId];
        }

        public override Task DeleteAsync(string id, IParent? parent)
        {
            GetListForParent(parent).RemoveAll(x => x.Id == id);

            return Task.CompletedTask;
        }

        public override Task<IEnumerable<TEntity>> GetAllAsync(IParent? parent, IQuery<TEntity> query)
        {
            var dataQuery = GetListForParent(parent).AsQueryable();

            dataQuery = query.ApplyDataView(dataQuery);
            dataQuery = query.ApplyOrder(dataQuery);

            if (query.SearchTerm != null)
            {
                // this is not a very useful search function, but it's just an example
                dataQuery = dataQuery.Where(x => x.Id.Contains(query.SearchTerm));
            }

            dataQuery = query.ApplyOrder(dataQuery);

            var data = dataQuery
                .Skip(query.Skip)
                .Take(query.Take)
                .ToList()
                .Select(x => (TEntity)x.Clone());

            query.HasMoreData(GetListForParent(parent).Count > (query.Skip + query.Take));

            return Task.FromResult(data);
        }

        public override Task<TEntity?> GetByIdAsync(string id, IParent? parent)
        {
            return Task.FromResult((TEntity?)GetListForParent(parent).FirstOrDefault(x => x.Id == id)?.Clone());
        }

        public override async Task<TEntity?> InsertAsync(IEditContext<TEntity> editContext)
        {
            editContext.Entity.Id = new Random().Next(0, int.MaxValue).ToString();

            await HandleRelationsAsync(editContext.Entity, editContext.GetRelationContainer());

            GetListForParent(editContext.Parent).Add(editContext.Entity);

            return (TEntity)editContext.Entity.Clone();
        }

        public override Task<TEntity> NewAsync(IParent? parent, Type? variantType = null)
        {
            return Task.FromResult(new TEntity());
        }

        // using editContext overload of UpdateAsync, you can access the state of the edit form
        // with this, you can check whether fields were edited (using editContext.IsModified(..)) 
        public override async Task UpdateAsync(IEditContext<TEntity> editContext)
        {
            var list = GetListForParent(editContext.Parent);

            var index = list.FindIndex(x => x.Id == editContext.Entity.Id);

            var newEntity = (TEntity)editContext.Entity.Clone();

            await HandleRelationsAsync(newEntity, editContext.GetRelationContainer());

            list.Insert(index, newEntity);
            list.RemoveAt(index + 1);
        }

        public override Task ReorderAsync(string? beforeId, string id, IParent? parent)
        {
            var parentId = parent?.Entity.Id ?? string.Empty;

            var entity = _data[parentId].FirstOrDefault(x => x.Id == id);
            if (entity == null)
            {
                return Task.CompletedTask;
            }

            _data[parentId].Remove(entity);
            if (string.IsNullOrWhiteSpace(beforeId))
            {
                _data[parentId].Add(entity);
            }
            else
            {
                var index = _data[parentId].FindIndex(x => x.Id == beforeId);
                _data[parentId].Insert(index, entity);
            }

            return Task.CompletedTask;
        }

        private async Task HandleRelationsAsync(TEntity entity, IRelationContainer? relations)
        {
            // this is some generic code to handle relations very genericly
            // please do not use in production (and you can't, since you cannot access IRepository)

            if (relations != null)
            {
                // use relations to process one-to-many relations between collections / tables
                // it contains a list of selected Ids, which should be used to update the relations

                foreach (var r in relations.Relations)
                {
                    try
                    {
                        if (r.Property is IFullPropertyMetadata fp)
                        {
                            var inMemoryRepo = _serviceProvider.GetService(typeof(InMemoryRepository<>).MakeGenericType(r.RelatedEntity)) as IRepository;
                            var jsonRepo = _serviceProvider.GetService(typeof(JsonRepository<>).MakeGenericType(r.RelatedEntity)) as IRepository;

                            var repo = inMemoryRepo ?? jsonRepo;

                            if (repo != null)
                            {
                                var relatedEntities = await r.RelatedElements
                                    .Select(x => x.Id.ToString())
                                    .ToListAsync(async id =>
                                    {
                                        var entity = await repo.GetByIdAsync(id, null);
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
