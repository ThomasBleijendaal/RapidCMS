using System;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Mediators;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Models.Data;
using RapidCMS.Core.Models.Results;

namespace RapidCMS.Core.Models.Queries
{
    public record GetEntitiesQuery(
        string CollectionAlias,
        string? VariantAlias,
        ParentPath? ParentPath,
        IRelated? Related,
        UsageType UsageType,
        [property: Obsolete]
        bool IsEmbedded,
        IView View) : IRequest<EntitiesResult>;
}
