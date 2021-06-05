using System.Threading.Tasks;

namespace RapidCMS.Core.Abstractions.Setup
{
    public interface ILogin
    {
        Task<ITypeRegistration?> CustomLoginScreenRegistrationAsync();
        Task<ITypeRegistration?> CustomLoginStatusRegistrationAsync();
    }
}
