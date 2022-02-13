using System.Threading.Tasks;
using RapidCMS.Core.Models.Setup;

namespace RapidCMS.Core.Abstractions.Setup
{
    public interface ILogin
    {
        /// <summary>
        /// Gets the type registration for the custom login screen for unauthenticated users.
        /// </summary>
        /// <returns></returns>
        Task<TypeRegistrationSetup?> CustomLoginScreenRegistrationAsync();

        /// <summary>
        /// Gets the type registration for the custom login status for in the top bar.
        /// </summary>
        /// <returns></returns>
        Task<TypeRegistrationSetup?> CustomLoginStatusRegistrationAsync();

        /// <summary>
        /// Gets the type registration for the custom landing page for when the user has no rights.
        /// </summary>
        /// <returns></returns>
        Task<TypeRegistrationSetup?> CustomLandingPageRegistrationAsync();
    }
}
