using System;

namespace RapidCMS.Core.Abstractions.Resolvers
{
    internal interface IRepositoryTypeResolver
    {
        public Type GetType(string repositoryAlias);
        public string GetAlias(Type originallyRegisterdType);
    }
}
