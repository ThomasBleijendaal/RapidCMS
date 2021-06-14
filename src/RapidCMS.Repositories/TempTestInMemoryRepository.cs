using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Forms;
using RapidCMS.Core.Abstractions.Mediators;
using RapidCMS.Core.Abstractions.Metadata;
using RapidCMS.Core.Abstractions.Repositories;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Extensions;
using RapidCMS.Core.Forms;
using RapidCMS.Core.Models.EventArgs.Mediators;
using RapidCMS.Core.Repositories;

namespace RapidCMS.Repositories
{
    /// <summary>
    /// This generic repository saves TEntities in memory and has some basic support for one-to-many relations.
    /// Use *only* List<TRelatedEntity> properties for relations.
    /// 
    /// NOTE: This repository messes up the lifetime of an entity so don't use this.
    /// </summary>
    /// <typeparam name="TEntity">Entity to store</typeparam>
    public class TempTestInMemoryRepository<TEntity> : BaseRepository<TEntity>
        where TEntity : class, IEntity, new()
    {
        protected Dictionary<string, List<TEntity>> _data = new Dictionary<string, List<TEntity>>();
        protected Dictionary<string, List<string>> _relations = new Dictionary<string, List<string>>();
        private readonly IMediator _mediator;
        private readonly IServiceProvider _serviceProvider;

        public TempTestInMemoryRepository(IMediator mediator, IServiceProvider serviceProvider)
        {
            _mediator = mediator;
            _serviceProvider = serviceProvider;
        }

        protected List<TEntity> GetListForParent(IParent? parent)
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

            _mediator.NotifyEvent(this, new MessageEventArgs(MessageType.Success, "Entity deleted."));

            return Task.CompletedTask;
        }

        public override Task<IEnumerable<TEntity>> GetAllAsync(IParent? parent, IQuery<TEntity> query)
        {
            var dataQuery = GetListForParent(parent).AsQueryable();

            dataQuery = query.ApplyDataView(dataQuery);

            if (query.SearchTerm != null)
            {
                var stringProperties = typeof(TEntity).GetProperties().Where(x => x.PropertyType == typeof(string)).ToList();

                // this is not a very fast or sensible search function, but it's just an example that works for all entities
                dataQuery = dataQuery
                    .Where(x => stringProperties
                        .Any(property => (property.GetValue(x) as string) != null ? ((string)property.GetValue(x)!).Contains(query.SearchTerm, StringComparison.InvariantCultureIgnoreCase) : false));
            }

            dataQuery = query.ApplyOrder(dataQuery);

            var dataQueryResult = dataQuery
                .Skip(query.Skip)
                .Take(query.Take + 1)
                .ToList();

            var data = dataQueryResult
                .Take(query.Take);

            query.HasMoreData(dataQueryResult.Count > query.Take);

            return Task.FromResult(data);
        }

        public override Task<IEnumerable<TEntity>?> GetAllRelatedAsync(IRelated related, IQuery<TEntity> query)
        {
            var ids = _relations.Where(x => x.Value.Contains(related.Entity.Id!)).Select(x => x.Key);

            return Task.FromResult(GetListForParent(related.Parent).Where(x => ids.Contains(x.Id!)))!;
        }

        public override Task<IEnumerable<TEntity>?> GetAllNonRelatedAsync(IRelated related, IQuery<TEntity> query)
        {
            var ids = _relations.Where(x => x.Value.Contains(related.Entity.Id!)).Select(x => x.Key);

            return Task.FromResult(GetListForParent(related.Parent).Where(x => !ids.Contains(x.Id!)))!;
        }

        public override async Task<TEntity?> GetByIdAsync(string id, IParent? parent)
        {
            var entity = (TEntity?)GetListForParent(parent).FirstOrDefault(x => x.Id == id);

            if (entity == null)
            {
                entity = (TEntity?)(await NewAsync(parent, default));
                entity!.Id = id;
            }

            return entity;
        }

        public override async Task<TEntity?> InsertAsync(IEditContext<TEntity> editContext)
        {
            editContext.Entity.Id = new Random().Next(0, int.MaxValue).ToString();

            await HandleRelationsAsync(editContext.Entity, editContext.GetRelationContainer());

            GetListForParent(editContext.Parent).Add(editContext.Entity);

            _mediator.NotifyEvent(this, new MessageEventArgs(MessageType.Success, "Entity created."));

            return (TEntity)editContext.Entity;
        }

        public override Task<TEntity> NewAsync(IParent? parent, Type? variantType = null)
        {
            if (variantType != null)
            {
                return Task.FromResult((TEntity)Activator.CreateInstance(variantType)!);
            }

            return Task.FromResult(new TEntity());
        }

        // using editContext overload of UpdateAsync, you can access the state of the edit form
        // with this, you can check whether fields were edited (using editContext.IsModified(..)) 
        public override async Task UpdateAsync(IEditContext<TEntity> editContext)
        {
            var list = GetListForParent(editContext.Parent);

            var newEntity = (TEntity)editContext.Entity;

            var index = list.FindIndex(x => x.Id == editContext.Entity.Id);
            if (index == -1)
            {
                list.Add(newEntity);
            }
            else
            {

                list.Insert(index, newEntity);
                list.RemoveAt(index + 1);
            }

            await HandleRelationsAsync(newEntity, editContext.GetRelationContainer());

            _mediator.NotifyEvent(this, new MessageEventArgs(MessageType.Success, "Entity updated."));
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

            _mediator.NotifyEvent(this, new MessageEventArgs(MessageType.Success, "Entities reordered."));

            return Task.CompletedTask;
        }

        public override Task AddAsync(IRelated related, string id)
        {
            if (!_relations.ContainsKey(id))
            {
                _relations.Add(id, new List<string>());
            }

            _relations[id].Add(related.Entity.Id!);

            _mediator.NotifyEvent(this, new MessageEventArgs(MessageType.Success, "Entity added."));

            return Task.CompletedTask;
        }

        public override Task RemoveAsync(IRelated related, string id)
        {
            if (!_relations.ContainsKey(id))
            {
                _relations.Add(id, new List<string>());
            }

            _relations[id].Remove(related.Entity.Id!);

            _mediator.NotifyEvent(this, new MessageEventArgs(MessageType.Success, "Entity removed."));

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

                foreach (var relation in relations.Relations)
                {
                    try
                    {
                        if (relation.Property is IFullPropertyMetadata fp)
                        {
                            // this is pretty ugly
                            var baseRepo = _serviceProvider.GetService(typeof(BaseRepository<>).MakeGenericType(relation.RelatedEntityType)) as IRepository;
                            var inMemoryRepo = _serviceProvider.GetService(typeof(TempTestInMemoryRepository<>).MakeGenericType(relation.RelatedEntityType)) as IRepository;

                            var repo = baseRepo ?? inMemoryRepo;

                            if (repo != null)
                            {
                                var relatedEntities = await relation.RelatedElementIds
                                    .Select(x => x?.ToString())
                                    .OfType<string>()
                                    .ToListAsync(async id =>
                                    {
                                        var entity = await repo.GetByIdAsync(id!, new ViewContext("", default));
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
