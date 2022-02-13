using System.Threading.Tasks;
using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Abstractions.Setup;
using RapidCMS.Core.Models.Config;

namespace RapidCMS.Core.Models.Setup
{
    internal class CmsSetup : ICms, ILogin
    {
        private readonly CmsConfig _config;
        private readonly ISetupResolver<TypeRegistrationSetup, CustomTypeRegistrationConfig> _typeRegistrationSetupResolver;

        public CmsSetup(
            CmsConfig config,
            ISetupResolver<TypeRegistrationSetup, CustomTypeRegistrationConfig> typeRegistrationSetupResolver)
        {
            _config = config;
            _typeRegistrationSetupResolver = typeRegistrationSetupResolver;

            SiteName = config.SiteName;
            IsDevelopment = config.IsDevelopment;
        }

        internal string SiteName { get; set; }
        internal bool IsDevelopment { get; set; }

        public TypeRegistrationSetup? CustomLoginScreenRegistration { get; internal set; }
        public TypeRegistrationSetup? CustomLoginStatusRegistration { get; internal set; }
        public TypeRegistrationSetup? CustomLandingPageRegistration { get; internal set; }

        string ICms.SiteName => SiteName;
        bool ICms.IsDevelopment
        {
            get => IsDevelopment;
            set => IsDevelopment = value;
        }

        public async Task<TypeRegistrationSetup?> CustomLoginScreenRegistrationAsync()
        {
            if (_config.CustomLoginScreenRegistration != null)
            {
                return (await _typeRegistrationSetupResolver.ResolveSetupAsync(_config.CustomLoginScreenRegistration)).Setup;
            }

            return default;
        }

        public async Task<TypeRegistrationSetup?> CustomLoginStatusRegistrationAsync()
        {
            if (_config.CustomLoginStatusRegistration != null)
            {
                return (await _typeRegistrationSetupResolver.ResolveSetupAsync(_config.CustomLoginStatusRegistration)).Setup;
            }

            return default;
        }

        public async Task<TypeRegistrationSetup?> CustomLandingPageRegistrationAsync()
        {
            if (_config.CustomLandingPageRegistration != null)
            {
                return (await _typeRegistrationSetupResolver.ResolveSetupAsync(_config.CustomLandingPageRegistration)).Setup;
            }

            return default;
        }
    }
}
