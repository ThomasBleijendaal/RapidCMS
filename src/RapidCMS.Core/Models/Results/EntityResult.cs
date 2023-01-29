using System.Collections.Generic;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Models.Setup;

namespace RapidCMS.Core.Models.Results
{
    public record EntityResult(
        string CollectionAlias,
        string RepositoryAlias, 
        string EntityVariantAlias,
        IEntity Entity,
        IParent? Parent,
        UsageType UsageType,
        List<ValidationSetup> Validators);
}
