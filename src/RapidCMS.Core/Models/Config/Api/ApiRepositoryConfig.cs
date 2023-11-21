using System;
using RapidCMS.Core.Abstractions.Config;

namespace RapidCMS.Core.Models.Config.Api;

internal class ApiRepositoryConfig : IApiRepositoryConfig
{
    public string Alias { get; set; } = default!;
    public Type EntityType { get; set; } = default!;
    public Type? DatabaseType { get; set; }
    public Type RepositoryType { get; set; } = default!;
    public Type ApiRepositoryType { get; set; } = default!;
}
