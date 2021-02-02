using System;

namespace RapidCMS.Core.Abstractions.Resolvers
{
    public interface IRepositoryTypeResolver
    {
        public Type GetType(string repositoryAlias);
        public (Type entityType, Type databaseType) GetEntityTypes(string repositoryAlias);
        public string GetAlias(Type originallyRegisterdType);
    }
}
