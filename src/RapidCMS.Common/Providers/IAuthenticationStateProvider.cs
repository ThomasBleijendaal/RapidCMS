using System.Threading.Tasks;

namespace RapidCMS.Common.Providers
{
    public interface IAuthenticationStateProvider
    {
        Task<bool> UserIsAuthenticatedAsync();
    }
}
