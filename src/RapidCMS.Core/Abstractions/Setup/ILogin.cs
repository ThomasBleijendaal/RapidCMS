using System.Threading.Tasks;

namespace RapidCMS.Core.Abstractions.Setup
{
    public interface ILogin
    {
        /// <summary>
        /// Gets the type registration for the custom login screen for unauthenticated users.
        /// </summary>
        /// <returns></returns>
        Task<ITypeRegistration?> CustomLoginScreenRegistrationAsync();

        /// <summary>
        /// Gets the type registration for the custom login status for in the top bar.
        /// </summary>
        /// <returns></returns>
        Task<ITypeRegistration?> CustomLoginStatusRegistrationAsync();

        /// <summary>
        /// Gets the type registration for the custom landing page for when the user has no rights.
        /// </summary>
        /// <returns></returns>
        Task<ITypeRegistration?> CustomLandingPageRegistrationAsync();
    }
}
