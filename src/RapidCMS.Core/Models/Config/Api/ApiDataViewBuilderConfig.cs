using System;
using RapidCMS.Core.Abstractions.Config;

namespace RapidCMS.Core.Models.Config.Api;

internal class ApiDataViewBuilderConfig : IApiDataViewBuilderConfig
{
    public string Alias { get; set; } = default!;

    public Type DataViewBuilder { get; set; } = default!;
}
