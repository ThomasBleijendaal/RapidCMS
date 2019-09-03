using System.Collections.Generic;
using RapidCMS.Common.Models;

namespace RapidCMS.Common.Providers
{
    public interface ICustomRegistrationProvider
    {
        IEnumerable<CustomTypeRegistration> CustomDashboardSectionRegistrations { get; }
        CustomTypeRegistration? CustomLoginRegistration { get; }
    }
}
