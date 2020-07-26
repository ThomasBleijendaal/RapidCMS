using System;
using RapidCMS.Core.Abstractions.Data;

namespace RapidCMS.Core.Abstractions.Config
{
    public interface IApiRepositoryConfig
    {
        string Alias { get; } 
        Type EntityType { get; }
        Type? DatabaseType { get; }
        Type RepositoryType { get;  } 
        Type? DataViewBuilderType { get; }

        /// <summary>
        /// Adds a data view builder to the repository. Data view builders allow for creating dynamic data views.
        /// </summary>
        /// <typeparam name="TDataViewBuilder"></typeparam>
        /// <returns></returns>
        IApiRepositoryConfig SetDataViewBuilder<TDataViewBuilder>() where TDataViewBuilder : IDataViewBuilder;
    }
}
