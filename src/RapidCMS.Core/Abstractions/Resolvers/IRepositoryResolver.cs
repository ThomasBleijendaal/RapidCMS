using System;
using RapidCMS.Core.Abstractions.Repositories;
using RapidCMS.Core.Models.Setup;

namespace RapidCMS.Core.Abstractions.Resolvers;

public interface IRepositoryResolver
{
    internal IRepository GetRepository(CollectionSetup collection);
    public IRepository GetRepository(string repositoryAlias);
    public IRepository GetRepository(Type repositoryType);
}
