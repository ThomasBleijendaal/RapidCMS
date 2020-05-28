using System;
using RapidCMS.Core.Abstractions.Config;
using RapidCMS.Core.Abstractions.Data;

namespace RapidCMS.Core.Models.Config.Api
{
    internal class ApiCollectionConfig : IApiCollectionConfig
    {
        public Type EntityType { get; set; } = default!;
        public Type? DatabaseType { get; set; }
        public Type RepositoryType { get; set; } = default!;
        public Type? DataViewBuilderType { get; set; }

        public IApiCollectionConfig SetDataViewBuilder<TDataViewBuilder>()
             where TDataViewBuilder : IDataViewBuilder
        {
            DataViewBuilderType = typeof(TDataViewBuilder);
            return this;
        }
    }
}
