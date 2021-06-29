using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Abstractions.Setup;

namespace RapidCMS.Api.Core.Resolvers
{
    internal class ValidationSetupResolver : ISetupResolver<IReadOnlyList<IValidationSetup>>
    {
        private readonly Dictionary<string, IReadOnlyList<IValidationSetup>> _validations;

        public ValidationSetupResolver(Dictionary<string, IReadOnlyList<IValidationSetup>> validations)
        {
            _validations = validations;
        }

        public Task<IReadOnlyList<IValidationSetup>> ResolveSetupAsync()
            => Task.FromResult<IReadOnlyList<IValidationSetup>>(_validations.Values.SelectMany(x => x).ToList());

        public Task<IReadOnlyList<IValidationSetup>> ResolveSetupAsync(string alias)
            => Task.FromResult(_validations.TryGetValue(alias, out var list) ? list : new List<IValidationSetup>());
    }
}
