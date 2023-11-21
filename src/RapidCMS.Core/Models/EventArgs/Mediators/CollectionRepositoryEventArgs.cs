using System;
using System.Collections.Generic;
using RapidCMS.Core.Abstractions.Mediators;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Models.Data;

namespace RapidCMS.Core.Models.EventArgs.Mediators;

public class CollectionRepositoryEventArgs : IMediatorEventArgs
{
    public CollectionRepositoryEventArgs(string collectionAlias, string repositoryAlias, ParentPath? parentPath, string? id, CrudType action)
    {
        CollectionAlias = collectionAlias ?? throw new ArgumentNullException(nameof(collectionAlias));
        RepositoryAlias = repositoryAlias ?? throw new ArgumentNullException(nameof(repositoryAlias));
        ParentPath = parentPath;
        Id = id;
        Action = action;
    }

    public CollectionRepositoryEventArgs(string collectionAlias, string repositoryAlias, ParentPath? parentPath, IEnumerable<string>? ids, CrudType action)
    {
        CollectionAlias = collectionAlias ?? throw new ArgumentNullException(nameof(collectionAlias));
        RepositoryAlias = repositoryAlias ?? throw new ArgumentNullException(nameof(repositoryAlias));
        ParentPath = parentPath;
        Ids = ids;
        Action = action;
    }

    public string CollectionAlias { get; set; }
    public string RepositoryAlias { get; set; }
    public ParentPath? ParentPath { get; set; }
    public string? Id { get; set; }
    public IEnumerable<string>? Ids { get; set; }

    public CrudType Action { get; set; }
}
