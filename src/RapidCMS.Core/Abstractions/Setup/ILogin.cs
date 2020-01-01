using RapidCMS.Core.Models.Setup;

namespace RapidCMS.Core.Abstractions.Setup
{
    internal interface ILogin
    {
        CustomTypeRegistrationSetup? CustomLoginScreenRegistration { get; }
        CustomTypeRegistrationSetup? CustomLoginStatusRegistration { get; }
    }
}
