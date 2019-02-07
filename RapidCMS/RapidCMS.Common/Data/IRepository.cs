using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RapidCMS.Common.Interfaces;

namespace RapidCMS.Common.Data
{
    public interface IRepository
    {
        IEnumerable<IEntity> GetAllAsObjects();
    }

    public abstract class BaseRepository<TKey, TEntity> : IRepository, IRepository<TKey, TEntity>
        where TEntity : IEntity
    {
        public abstract IEnumerable<TEntity> GetAll();
        public abstract TEntity GetById(TKey id);

        IEnumerable<IEntity> IRepository.GetAllAsObjects()
        {
            return GetAll().Cast<IEntity>();
        }
    }

    public interface IRepository<TKey, TEntity> : IRepository
        where TEntity : IEntity
    {
        TEntity GetById(TKey id);
        IEnumerable<TEntity> GetAll();
    }
}
