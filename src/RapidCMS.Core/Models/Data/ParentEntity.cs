using RapidCMS.Core.Abstractions.Data;

namespace RapidCMS.Core.Models.Data;

internal class ParentEntity : IParent
{
    private readonly string _repositoryAlias;

    public ParentEntity(IParent? parent, IEntity entity, string repositoryAlias)
    {
        Parent = parent;
        Entity = entity;
        _repositoryAlias = repositoryAlias;
    }

    public IParent? Parent { get; }
    public IEntity Entity { get; private set; }

    public ParentPath? GetParentPath()
    {
        return ParentPath.AddLevel(Parent?.GetParentPath(), _repositoryAlias, Entity.Id!);
    }
}
