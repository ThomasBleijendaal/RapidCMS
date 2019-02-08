using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RapidCMS.Common.Interfaces;

namespace RapidCMS.Common.Data
{
    public interface IRepository
    {
        Task<IEnumerable<IEntity>> GetAllAsObjectsAsync(int? parentId);
    }
    
    public interface IRepository<TKey, TEntity> : IRepository
        where TEntity : IEntity
    {
        Task<TEntity> GetByIdAsync(TKey id, int? parentId);
        Task<IEnumerable<TEntity>> GetAllAsync(int? parentId);
    }

    // TODO: add TKey? awesomeness

    public abstract class BaseRepository<TKey, TEntity> : IRepository, IRepository<TKey, TEntity>
        where TEntity : IEntity
    {
        public abstract Task<TEntity> GetByIdAsync(TKey id, int? parentId);
        public abstract Task<IEnumerable<TEntity>> GetAllAsync(int? parentId);

        async Task<IEnumerable<IEntity>> IRepository.GetAllAsObjectsAsync(int? parentId)
        {
            return (await GetAllAsync(parentId)).Cast<IEntity>();
        }
    }
}
