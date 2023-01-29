using RapidCMS.Core.Abstractions.Mediators;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Models.Data;
using RapidCMS.Core.Models.Results;

namespace RapidCMS.Core.Models.Queries
{
    public record GetEntityQuery(
        string CollectionAlias, 
        string? VariantAlias, 
        string? Id, 
        ParentPath? ParentPath, 
        UsageType UsageType)  : IRequest<EntityResult>;
}
