using System.Collections.Generic;
using RapidCMS.Core.Models.Setup;

namespace RapidCMS.Core.Abstractions.Setup
{
    internal interface IDashboard
    {
        IEnumerable<CustomTypeRegistrationSetup> CustomDashboardSectionRegistrations { get; }
    }
}
