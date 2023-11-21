namespace RapidCMS.Core.Abstractions.Data;

public interface IRelated
{
    IParent? Parent { get; }
    IEntity Entity { get; }

    string RepositoryAlias { get; }
}
