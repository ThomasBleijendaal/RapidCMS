using System.Collections.Generic;
using RapidCMS.Common.Models;

namespace RapidCMS.Common.Providers
{
    public interface ICustomRegistrationProvider
    {
        IEnumerable<CustomTypeRegistration> CustomButtonRegistrations { get; }
        IEnumerable<CustomTypeRegistration> CustomEditorRegistrations { get; }
        IEnumerable<CustomTypeRegistration> CustomSectionRegistrations { get; }
        IEnumerable<CustomTypeRegistration> CustomDashboardSectionRegistrations { get; }
        CustomTypeRegistration? CustomLoginRegistration { get; }
    }
}
