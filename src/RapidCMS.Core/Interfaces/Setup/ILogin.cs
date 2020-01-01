using RapidCMS.Core.Models.Setup;

namespace RapidCMS.Core.Interfaces.Setup
{
    internal interface ILogin
    {
        CustomTypeRegistrationSetup? CustomLoginScreenRegistration { get; }
        CustomTypeRegistrationSetup? CustomLoginStatusRegistration { get; }
    }
}
