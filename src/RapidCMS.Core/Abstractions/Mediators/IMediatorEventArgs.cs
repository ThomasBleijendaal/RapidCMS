using RapidCMS.Core.Models.Data;

namespace RapidCMS.Core.Abstractions.Mediators
{
    public interface IMediatorEventArgs
    {
        public ParentPath? ParentPath { get; }
    }
}
