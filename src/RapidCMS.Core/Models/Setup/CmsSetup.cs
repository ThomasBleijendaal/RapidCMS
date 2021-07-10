using System.Threading.Tasks;
using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Abstractions.Setup;
using RapidCMS.Core.Models.Config;

namespace RapidCMS.Core.Models.Setup
{
    internal class CmsSetup : ICms, ILogin
    {
        private readonly CmsConfig _config;
        private readonly ISetupResolver<ITypeRegistration, CustomTypeRegistrationConfig> _typeRegistrationSetupResolver;

        public CmsSetup(
            CmsConfig config,
            ISetupResolver<ITypeRegistration, CustomTypeRegistrationConfig> typeRegistrationSetupResolver)
        {
            _config = config;
            _typeRegistrationSetupResolver = typeRegistrationSetupResolver;

            SiteName = config.SiteName;
            IsDevelopment = config.IsDevelopment;
        }

        internal string SiteName { get; set; }
        internal bool IsDevelopment { get; set; }

        public ITypeRegistration? CustomLoginScreenRegistration { get; internal set; }
        public ITypeRegistration? CustomLoginStatusRegistration { get; internal set; }
        public ITypeRegistration? CustomLandingPageRegistration { get; internal set; }

        string ICms.SiteName => SiteName;
        bool ICms.IsDevelopment
        {
            get => IsDevelopment;
            set => IsDevelopment = value;
        }

        public async Task<ITypeRegistration?> CustomLoginScreenRegistrationAsync()
        {
            if (_config.CustomLoginScreenRegistration != null)
            {
                return (await _typeRegistrationSetupResolver.ResolveSetupAsync(_config.CustomLoginScreenRegistration)).Setup;
            }

            return default;
        }

        public async Task<ITypeRegistration?> CustomLoginStatusRegistrationAsync()
        {
            if (_config.CustomLoginStatusRegistration != null)
            {
                return (await _typeRegistrationSetupResolver.ResolveSetupAsync(_config.CustomLoginStatusRegistration)).Setup;
            }

            return default;
        }

        public async Task<ITypeRegistration?> CustomLandingPageRegistrationAsync()
        {
            if (_config.CustomLandingPageRegistration != null)
            {
                return (await _typeRegistrationSetupResolver.ResolveSetupAsync(_config.CustomLandingPageRegistration)).Setup;
            }

            return default;
        }
    }
}
