using System.Collections.Generic;

namespace RapidCMS.Core.Abstractions.Setup
{
    public interface IDashboard
    {
        IEnumerable<ITypeRegistration> CustomDashboardSectionRegistrations { get; }
    }
}
