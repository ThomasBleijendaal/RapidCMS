using RapidCMS.Core.Models.Data;

namespace RapidCMS.Core.Interfaces.Data
{
    public interface IParent
    {
        IParent? Parent { get; }
        IEntity Entity { get; }

        ParentPath? GetParentPath();
    }
}
