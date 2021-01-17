using System;
using RapidCMS.Core.Models.Data;

namespace RapidCMS.Core.Abstractions.Mediators
{
    public interface IMediatorEventArgs
    {
        [Obsolete("Remove from interface")]
        public ParentPath? ParentPath { get; }
    }
}
