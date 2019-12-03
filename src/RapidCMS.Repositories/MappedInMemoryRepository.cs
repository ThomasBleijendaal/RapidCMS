using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RapidCMS.Common.Data;
using RapidCMS.Common.Forms;

namespace RapidCMS.Repositories
{
    /// <summary>
    /// This generic repository saves TEntities in memory and has some basic support for one-to-many relations.
    /// No support for relations.
    /// 
    /// This MappedInMemoryRepository maps the objects to and from a different entity, allowing for using a different type of entity
    /// to be used for the database than which is used in the CMS view. This is usefull when the database entity is incompatible with
    /// the CMS, or if validation attributes cannot be placed on the database entities.
    /// 
    /// The IQuery's are not affected by this mapping, allowing you to define query expressions using the database entities, greatly
    /// simplifying the use of data views in these repositories.
    /// </summary>
    /// <typeparam name="TCmsEntity">Entity to store</typeparam>
    public class MappedInMemoryRepository<TCmsEntity, TEntity> : MappedBaseRepository<string, TCmsEntity, TEntity>
        where TCmsEntity : class, IEntity, new()
        where TEntity : class, IEntity, ICloneable, new()
    {
        protected Dictionary<string, List<TEntity>> _data = new Dictionary<string, List<TEntity>>();
        private readonly IConverter<TCmsEntity, TEntity> _converter;

        public MappedInMemoryRepository(IConverter<TCmsEntity, TEntity> converter)
        {
            _converter = converter;
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

        public override Task<IEnumerable<TCmsEntity>> GetAllAsync(IParent? parent, IQuery<TEntity> query)
        {
            var dataQuery = GetListForParent(parent).AsEnumerable();

            if (query.DataViewExpression != null)
            {
                dataQuery = dataQuery.Where(query.DataViewExpression.Compile());
            }

            if (query.SearchTerm != null)
            {
                // this is not a very useful search function, but it's just an example
                dataQuery = dataQuery.Where(x => x.Id?.Contains(query.SearchTerm) ?? false);
            }

            var data = dataQuery
                .Skip(query.Skip)
                .Take(query.Take)
                .Select(x => _converter.Convert((TEntity)x.Clone()));

            query.HasMoreData(GetListForParent(parent).Count > (query.Skip + query.Take));

            return Task.FromResult(data); 
        }

        public override Task<TCmsEntity?> GetByIdAsync(string id, IParent? parent)
        {
            var entity = (TEntity?)GetListForParent(parent).FirstOrDefault(x => x.Id == id)?.Clone();
            if (entity == null)
            {
                return Task.FromResult(default(TCmsEntity));
            }
            
            return Task.FromResult(_converter.Convert(entity))!;
        }

        public override Task<TCmsEntity?> InsertAsync(IEditContext<TCmsEntity> editContext)
        {
            editContext.Entity.Id = new Random().Next(0, int.MaxValue).ToString();

            var entity = _converter.Convert(editContext.Entity);

            GetListForParent(editContext.Parent).Add(entity);

            return Task.FromResult( _converter.Convert((TEntity)entity.Clone()))!;
        }

        public override Task<TCmsEntity> NewAsync(IParent? parent, Type? variantType = null)
        {
            return Task.FromResult(new TCmsEntity());
        }

        public override string ParseKey(string id)
        {
            return id;
        }

        public override Task UpdateAsync(IEditContext<TCmsEntity> editContext)
        {
            var list = GetListForParent(editContext.Parent);

            var index = list.FindIndex(x => x.Id == editContext.Entity.Id);

            var newEntity = (TEntity)_converter.Convert(editContext.Entity).Clone();

            list.Insert(index, newEntity);
            list.RemoveAt(index + 1);

            return Task.CompletedTask;
        }
    }
}
