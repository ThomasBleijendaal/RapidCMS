using System.Collections.Generic;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Models.Setup;

namespace RapidCMS.Core.Interfaces.Setup
{
    public interface IButton
    {
        CrudType? DefaultCrudType { get; }

        string Label { get; }
        string Icon { get; }

        IEntityVariant? EntityVariant { get; }
    }

    internal interface ICms
    {
        string SiteName { get; }
        bool IsDevelopment { get; }
        bool AllowAnonymousUsage { get; }

        int SemaphoreMaxCount { get; }
    }

    internal interface ICollections
    {
        IEnumerable<CollectionSetup> Collections { get; }
    }

    internal interface IDashboard
    {
        IEnumerable<CustomTypeRegistrationSetup> CustomDashboardSectionRegistrations { get; }
    }

    internal interface ILogin
    {
        CustomTypeRegistrationSetup? CustomLoginScreenRegistration { get; set; }
        CustomTypeRegistrationSetup? CustomLoginStatusRegistration { get; set; }
    }
}
