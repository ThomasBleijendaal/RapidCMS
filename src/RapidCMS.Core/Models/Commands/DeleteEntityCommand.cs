using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Mediators;

namespace RapidCMS.Core.Models.Commands
{
    public record DeleteEntityCommand(
        string RepositoryAlias,
        IEntity Entity,
        IParent? Parent) : IRequest;

    public record UpdateEntityCommand(
        string RepositoryAlias,
        IEntity Entity,
        IParent? Parent) : IRequest;
}
