using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using RapidCMS.Common.Data;
using RapidCMS.Common.Interfaces;

namespace RapidCMS.Common.Models.Config
{
    public class CollectionConfig<TEntity>
        where TEntity : IEntity
    {
        internal Type RepositoryType { get; set; }   

        public TreeViewConfig<TEntity> TreeView { get; set; }

        public CollectionConfig<TEntity> SetRepository<TRepository>()
           where TRepository : IRepository<int, TEntity>
        {
            RepositoryType = typeof(TRepository);

            return this;
        }

        public TreeViewConfig<TEntity> SetTreeView(string name, ViewType viewType, Expression<Func<TEntity, string>> expression)
        {
            TreeView = new TreeViewConfig<TEntity>
            {
                Name = name,
                ViewType = viewType,
                NameGetter = expression
            };

            return TreeView;
        }
    }

    public class TreeViewConfig<TEntity>
        where TEntity : IEntity
    {
        public string Name { get; set; }
        public ViewType ViewType { get; set; }
        public Expression<Func<TEntity, string>> NameGetter { get; set; }
    }
}
