﻿using System;
using RapidCMS.Core.Abstractions.Validators;

namespace RapidCMS.Core.Abstractions.Config;

public interface IApiRepositoryConfig
{
    string Alias { get; }
    Type EntityType { get; }
    Type? DatabaseType { get; }
    Type RepositoryType { get; }
    Type ApiRepositoryType { get; }
}
