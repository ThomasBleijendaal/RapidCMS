using System;
using RapidCMS.Core.Abstractions.Mediators;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Models.Data;

namespace RapidCMS.Core.Models.EventArgs.Mediators
{
    public class RepositoryEventArgs : IMediatorEventArgs
    {
        public RepositoryEventArgs(Type repositoryType, ParentPath? parentPath, string? id, CrudType action)
        {
            RepositoryType = repositoryType ?? throw new ArgumentNullException(nameof(repositoryType));
            ParentPath = parentPath;
            Id = id;
            Action = action;
        }

        public Type RepositoryType { get; set; }
        public ParentPath? ParentPath { get; set; }
        public string? Id { get; set; }
        public CrudType Action { get; set; }
    }
}
