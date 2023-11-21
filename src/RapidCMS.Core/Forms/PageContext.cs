using RapidCMS.Core.Abstractions.Data;

namespace RapidCMS.Core.Forms;

public sealed class PageContext
{
    internal PageContext(
        string pageAlias,
        IEntity? entity,
        IParent? parent)
    {
        PageAlias = pageAlias;
        Entity = entity;
        Parent = parent;
    }

    public string PageAlias { get; private set; }
    public IEntity? Entity { get; private set; }
    public IParent? Parent { get; private set; }
}
