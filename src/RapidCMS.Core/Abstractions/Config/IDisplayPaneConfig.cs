﻿using System;
using System.Linq.Expressions;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Repositories;

namespace RapidCMS.Core.Abstractions.Config;

public interface IDisplayPaneConfig<TEntity> : IHasButtons<IDisplayPaneConfig<TEntity>>
    where TEntity : IEntity
{
    /// <summary>
    /// Adds a field to the pane.
    /// </summary>
    /// <param name="displayExpression">Expression to display this field</param>
    /// <param name="configure">Action to configure this field</param>
    /// <returns></returns>
    IDisplayFieldConfig<TEntity, string> AddField(Expression<Func<TEntity, string?>> displayExpression, Action<IDisplayFieldConfig<TEntity, string>>? configure = null);

    /// <summary>
    /// Adds a sub collection to the pane. A sub collection is defined separately, and only referenced by alias.
    /// 
    /// Not visible when EntityState is New.
    /// </summary>
    /// <param name="collectionAlias">Alias of the sub collection</param>
    /// <returns></returns>
    IDisplayPaneConfig<TEntity> AddSubCollectionList(string collectionAlias);

    /// <summary>
    /// Adds a sub collection to the pane. A sub collection is defined separately, and only referenced by alias.
    /// 
    /// Not visible when EntityState is New.
    /// </summary>
    /// <typeparam name="TSubEntity">Type of the sub collections entity</typeparam>
    /// <typeparam name="TSubRepository">Type of the sub repository</typeparam>
    /// <param name="configure">Action to configure the use of this sub collection</param>
    /// <returns></returns>
    IDisplayPaneConfig<TEntity> AddSubCollectionList<TSubEntity, TSubRepository>(Action<ISubCollectionListViewConfig<TSubEntity, TSubRepository>>? configure = null)
        where TSubRepository : IRepository
        where TSubEntity : IEntity;

    /// <summary>
    /// Adds a collection to the pane which is used to view the many-to-many relation between the collection of this pane, and the related collection.
    /// The related collection can by any collection.
    /// 
    /// Not visible when EntityState is New.
    /// </summary>
    /// <param name="collectionAlias">Alias of the related collection</param>
    /// <returns></returns>
    IDisplayPaneConfig<TEntity> AddRelatedCollectionList(string collectionAlias);

    /// <summary>
    /// Adds a collection to the pane which is used to view the many-to-many relation between the collection of this pane, and the related collection.
    /// The related collection can by any collection.
    /// 
    /// Not visible when EntityState is New.
    /// </summary>
    /// <typeparam name="TRelatedEntity">Type of the related collections entity</typeparam>
    /// <typeparam name="TRelatedRepository">Repository for the related collections</typeparam>
    /// <param name="configure">Action to configure the related collection</param>
    /// <returns></returns>
    IDisplayPaneConfig<TEntity> AddRelatedCollectionList<TRelatedEntity, TRelatedRepository>(Action<IRelatedCollectionListViewConfig<TRelatedEntity, TRelatedRepository>>? configure = null)
        where TRelatedRepository : IRepository
        where TRelatedEntity : IEntity;

    /// <summary>
    /// Adds a label at the top of this pane.
    /// </summary>
    /// <param name="label">Text to display in the label</param>
    /// <returns></returns>
    IDisplayPaneConfig<TEntity> SetLabel(string label);

    /// <summary>
    /// Expression which determines whether this pane should be visible.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    IDisplayPaneConfig<TEntity> VisibleWhen(Func<TEntity, EntityState, bool> predicate);
}
