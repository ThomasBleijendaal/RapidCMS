using RapidCMS.Core.Enums;

namespace RapidCMS.Core.Abstractions.Config;

internal interface IIsConventionBased
{
    Features GetFeatures();
}
