using System;

namespace RapidCMS.Core.Abstractions.Config
{
    public interface IApiDataViewBuilderConfig
    {
        string Alias { get; }
        Type DataViewBuilder { get; }
    }
}
